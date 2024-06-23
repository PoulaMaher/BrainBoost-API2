using AutoMapper;
using BrainBoost_API.DTOs.Teacher;
using BrainBoost_API.DTOs.Uploader;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public TeacherController(IUnitOfWork _unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            unitOfWork = _unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpPost("AddTeacher")]
        public async Task<IActionResult> AddTeacher([FromForm]insertedTeacher insertedTeacher)
        {
            ApplicationUser applicationUser = await userManager.FindByIdAsync(insertedTeacher.userId);
            if (applicationUser != null)
            {
                Teacher teacher = new Teacher()
                {
                    YearsOfExperience = insertedTeacher.yearsOfExperience,
                    Career = insertedTeacher.jobTitles,
                    AboutYou = insertedTeacher.aboutYou,
                    UserId = insertedTeacher.userId,
                    Fname = applicationUser.Fname,
                    Lname = applicationUser.Lname,
                    NumOfFollowers = 0
                };
                string photoUrl = await Uploader.uploadPhoto(insertedTeacher.photo, teacher.GetType().Name,teacher.Fname +" "+teacher.Lname );
                teacher.PictureUrl = photoUrl;
                unitOfWork.TeacherRepository.add(teacher);
                unitOfWork.save();
                return Ok(insertedTeacher);
            }
            return NotFound("User is not Valid");
        }
        [HttpGet("{id}")]
        public IActionResult GetTeacherById(int id)
        {
            Teacher teacher = unitOfWork.TeacherRepository.GetTeacherById(id);
            return Ok(teacher);
        }

        [HttpGet("GetCoursesOfTeacherById")]
        public IActionResult GetCoursesForTeacher(int TeacherId)
        {
            List<Course> Courses = unitOfWork.TeacherRepository.GetCoursesForTeacher(TeacherId);
            return Ok(Courses);
        }

        [HttpGet("GetTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            if (ModelState.IsValid)
            {
                List<Teacher> teachers = unitOfWork.TeacherRepository.GetAll().ToList();
                List<TeacherDataDTO> teachersData = new List<TeacherDataDTO>();
                foreach (Teacher teacher in teachers)
                {
                    TeacherDataDTO teacherData = mapper.Map<TeacherDataDTO>(teacher);
                    teachersData.Add(teacherData);
                }
                return Ok(teachersData);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteTeacher")]
        public IActionResult DeleteTeacher(int teacherId)
        {
            if (ModelState.IsValid)
            {
                var teacher = unitOfWork.TeacherRepository.Get(c => c.Id == teacherId);
                teacher.IsDeleted = true;
                unitOfWork.TeacherRepository.remove(teacher);
                unitOfWork.save();
                return Ok("Successfully Deleted");
            }
            return BadRequest(ModelState);
        }

        [HttpPut("UpdateTeacherData")]
        public IActionResult UpdateTeacherData(TeacherDataDTO updatedTeacher)
        {
            if (ModelState.IsValid)
            {
                Teacher teacher = mapper.Map<Teacher>(updatedTeacher);
                unitOfWork.TeacherRepository.update(teacher);
                unitOfWork.save();
                return Ok("Successfully Updated");
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetTopTeachers")]
        public async Task<IActionResult> GetTopTeachers()
        {
            if (ModelState.IsValid)
            {
                List<Teacher> teachers = unitOfWork.TeacherRepository.GetTopTeachers();
                List<TeacherDataDTO> teachersData = new List<TeacherDataDTO>();
                foreach (Teacher teacher in teachers)
                {
                    TeacherDataDTO teacherData = mapper.Map<TeacherDataDTO>(teacher);
                    teachersData.Add(teacherData);
                }
                return Ok(teachersData);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetTotalNumOfTeachers")]
        public IActionResult GetTotalNumOfTeachers()
        {
            int numOfTeachers = unitOfWork.TeacherRepository.GetTotalNumOfTeachers();
            return Ok(numOfTeachers);
        }
    }
}
