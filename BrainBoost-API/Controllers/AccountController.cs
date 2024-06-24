﻿using BrainBoost_API.DTOs;
using BrainBoost_API.DTOs.Account;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> UserManager;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IConfiguration Configuration;
        public readonly RoleManager<ApplicationRole> RoleManager;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager , IUnitOfWork unitOfWork ,IConfiguration configuration)
        {
            this.UserManager = userManager;
            this.UnitOfWork = unitOfWork;
            this.Configuration = configuration;
            this.RoleManager = roleManager;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterUserDto registerUser, string role)
        { 
            if (ModelState.IsValid)
            {
                if (!await RoleManager.RoleExistsAsync(role))
                {
                    ModelState.AddModelError("Role", role + " Role does not exist.");
                    return BadRequest(ModelState);
                }
                var user = new ApplicationUser
                {
                    UserName = registerUser.UserName,
                    Email = registerUser.Email,
                    Fname = registerUser.FirstName,
                    Lname = registerUser.LastName,
                };
                IdentityResult result = await UserManager.CreateAsync(user, registerUser.Password);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, role);
                    //switch (role)
                    //{
                    //    case "Student":
                    //        var student = new Student
                    //        {
                    //            AppUser = user,
                    //            Fname= registerUser.FirstName,
                    //            Lname= registerUser.LastName,
                    //        };
                    //        this.UnitOfWork.StudentRepository.add(student);
                    //        break;

                    //    case "Teacher":
                    //        var teacher = new Teacher
                    //        {
                    //            AppUser = user,
                    //            Fname = registerUser.FirstName,
                    //            Lname = registerUser.LastName,
                    //        };
                    //        this.UnitOfWork.TeacherRepository.add(teacher);
                    //        break;

                    //    default:
                    //        // Handle default case if necessary
                    //        break;
                    //}
                    this.UnitOfWork.save();
                    return Ok(new { Msg = "Account Created", userId=user.Id,role=role });
                }
                return BadRequest(new { Msg = result.Errors });
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = await UserManager.FindByNameAsync(loginUser.UserName);
                if(userFromDb != null)
                {
                    bool found = await UserManager.CheckPasswordAsync(userFromDb, loginUser.Password);
                    if (found)
                    {
                        List<Claim> userClaims = new List<Claim>();
                        userClaims.Add(new Claim(ClaimTypes.Name, userFromDb.UserName));
                        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userFromDb.Id));
                        userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var roles = await UserManager.GetRolesAsync(userFromDb);
                        foreach (var role in roles)
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]));
                        SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        //create Token
                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: Configuration["JWT:ValidIssuer"],
                            audience: Configuration["JWT:ValidAudience"],
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: signingCredentials,
                            claims: userClaims
                        );
                        return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token), expired=token.ValidTo});
                    }

                }
                return Unauthorized(new { Msg = "Invalid Account" });
            }
            return BadRequest(ModelState);
        }
    }
}
