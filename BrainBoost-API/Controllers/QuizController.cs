using BrainBoost_API.DTOs.Course;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;

        public QuizController(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }

        [HttpGet("ChangeQuizState/{id:int}")]
        public async Task<IActionResult> ChangeQuizState(int id, [FromQuery] string status)
        {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = UnitOfWork.StudentRepository.Get(c => c.UserId == UserID);

                var enrolledCourse = UnitOfWork.StudentEnrolledCoursesRepository.Get(c => c.StudentId == std.Id && c.CourseId == id);
                enrolledCourse.QuizState = true;
                UnitOfWork.StudentEnrolledCoursesRepository.update(enrolledCourse);
                UnitOfWork.save();

                return Ok();
            }
            return BadRequest(ModelState);
        }
        
    }
}
