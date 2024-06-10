using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public TeacherController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
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
                return Ok(teachers);
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
    }
}
