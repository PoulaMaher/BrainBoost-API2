using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; }
        public StudentController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            IEnumerable<Student> students = UnitOfWork.StudentRepository.GetAll();
            if (students == null)
            {
                return NotFound();
            }
            return Ok(students);
        }


        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            Student student = UnitOfWork.StudentRepository.Get(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Student student = UnitOfWork.StudentRepository.Get(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            student.IsDeleted = true;
            UnitOfWork.StudentRepository.remove(student);
            UnitOfWork.save();
            return Ok("Student Deleted Successfully!");
        }
    }
}
