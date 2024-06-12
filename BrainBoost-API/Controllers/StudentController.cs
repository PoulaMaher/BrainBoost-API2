using AutoMapper;
using BrainBoost_API.DTOs.Student;
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
        public IMapper Mapper { get; }

        public StudentController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
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

        [HttpGet("GetTotalNumOfStudent")]
        public IActionResult GetTotalNumOfStudent()
        {
            int numofstudent = UnitOfWork.StudentRepository.GetTotalNumOfStudent();
            return Ok(numofstudent);
        }

        [HttpPut("Update")]
        public IActionResult Update(StudentDTO studentDTO, int id)
        {
            Student studentfromDB = UnitOfWork.StudentRepository.Get(s => s.Id == id);
            if (studentfromDB == null)
            {
                return NotFound();
            }

            if (studentDTO.Id == id && ModelState.IsValid)
            {
                Mapper.Map(studentDTO, studentfromDB);
                UnitOfWork.StudentRepository.update(studentfromDB);
                UnitOfWork.save();
                return Ok("Data Updated Successfully");
            }
            return BadRequest(ModelState);

        }
    }
}
