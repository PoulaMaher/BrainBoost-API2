using AutoMapper;
using BrainBoost_API.DTOs.Admin;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public AdminController(IUnitOfWork _unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            unitOfWork = _unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet("GetAllAdmins")]
        public IActionResult GetAllAdmins()
        {
            if (ModelState.IsValid)
            {
                List<Admin> admins = unitOfWork.AdminRepository.GetAll().ToList();
                List<AdminDTO> adminsData = new List<AdminDTO>();
                foreach (Admin admin in admins)
                {
                    AdminDTO adminData = mapper.Map<AdminDTO>(admin);
                    adminsData.Add(adminData);
                }
                return Ok(adminsData);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("GetAdminById/{id}")]
        public IActionResult GetAdminById(int id)
        {
            Admin admin = unitOfWork.AdminRepository.Get(a => a.Id == id);

            if (admin == null)
            {
                return NotFound();
            }
            AdminDTO adminData = new AdminDTO();
            adminData = mapper.Map<AdminDTO>(admin);
            return Ok(adminData);
        }

        [HttpDelete("DeleteAdmin/{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            Admin admin = unitOfWork.AdminRepository.Get(a => a.Id == id);
            if (admin == null)
            {
                return NotFound();
            }
            admin.IsDeleted = true;
            unitOfWork.AdminRepository.remove(admin);
            unitOfWork.save();
            return Ok("Admin Deleted Successfully!");
        }
        [HttpPut("UpdateAdminData")]
        public IActionResult UpdateAdminData(AdminDTO updatedAdmin)
        {
            if (ModelState.IsValid)
            {
                Admin admin = mapper.Map<Admin>(updatedAdmin);
                unitOfWork.AdminRepository.update(admin);
                unitOfWork.save();
                return Ok("Successfully Updated");
            }
            return BadRequest(ModelState);
        }
    }
}
