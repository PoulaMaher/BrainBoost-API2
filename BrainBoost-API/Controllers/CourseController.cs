using AutoMapper;
using BrainBoost_API.DTOs.Course;
using BrainBoost_API.DTOs.Quiz;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper mapper;
        public CourseController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            this.UserManager = userManager;
            this.UnitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            if (ModelState.IsValid)
            {
                List<Course> Courses = UnitOfWork.CourseRepository.GetAll().ToList();
                return Ok(Courses);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetCourse/{id:int}")]
        public async Task<IActionResult> GetCourseDetails(int id)
        {
            if (ModelState.IsValid)
            {
                Course Course = UnitOfWork.CourseRepository.Get(c => c.Id == id, "Teacher,WhatToLearn");
                var review = UnitOfWork.ReviewRepository.GetList(r => r.CourseId == id).ToList();
                var numOfRates = UnitOfWork.ReviewRepository.GetList(r => r.CourseId == id).ToList().Count();
                var numOfVideos = UnitOfWork.VideoRepository.GetList(r => r.CrsId == id).ToList().Count();

                if (review.Count() > 4)
                {
                    review = UnitOfWork.ReviewRepository.GetList(r => r.CourseId == id).Take(4).ToList();
                }

                CourseDetailsDto crsDetails = UnitOfWork.CourseRepository.getCrsDetails(Course, review);
                crsDetails.NumOfRates = numOfRates;
                crsDetails.NumOfVideos = numOfVideos;
                crsDetails.TeacherDataDto.Email = UnitOfWork.TeacherRepository.Get(t => t.Id == Course.TeacherId, "AppUser").AppUser.Email;


                return Ok(crsDetails);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetCourseQuiz/{id:int}")]
        public async Task<IActionResult> GetCourseQuiz(int id)

        {
            if (ModelState.IsValid)
            {

                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = UnitOfWork.StudentRepository.Get(c => c.UserId == UserID);

                var enrolledCourse = UnitOfWork.StudentEnrolledCoursesRepository.Get(c => c.StudentId == std.Id && c.CourseId == id);
                bool IsTaken = enrolledCourse.QuizState;
                var Course = UnitOfWork.CourseRepository.Get(c => c.Id == id, "Teacher,WhatToLearn,videos,quiz");

                var quiz = Course.quiz;
                var quizQuestions = UnitOfWork.QuizRepository.Get(c => c.Id == quiz.Id, "Questions").Questions;
                var questionIds = quizQuestions.Select(q => q.QuestionId).ToList();
                var questions = UnitOfWork.QuestionRepository.GetList(r => questionIds.Contains(r.Id), "Answers").ToList();

                QuizDTO TakenQuiz = UnitOfWork.QuizRepository.getCrsQuiz(quiz, questions, IsTaken);

                return Ok(TakenQuiz);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse(Course NewCourse)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork.CourseRepository.add(NewCourse);
                UnitOfWork.save();
                return Ok(NewCourse);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetAllCoursesAsCards")]
        public ActionResult<List<CourseCardDataDto>> GetAllCoursesAsCards()
        {
            List<Course> courses = UnitOfWork.CourseRepository.GetAll().ToList();
            List<CourseCardDataDto> courseCards = new List<CourseCardDataDto>();
            foreach (Course course in courses)
            {
                CourseCardDataDto currentCourseCard = mapper.Map<CourseCardDataDto>(course);
                courseCards.Add(currentCourseCard);
            }
            return Ok(courseCards);
        }

        [HttpGet("GetFilteredCourses")]
        public ActionResult<List<CourseCardDataDto>> GetFilteredCourses([FromQuery] CourseFilterationDto filter)
        {
            List<Course> courses = UnitOfWork.CourseRepository.GetFilteredCourses(filter, "Category,Teacher").ToList();
            List<CourseCardDataDto> filteredCourseCards = new List<CourseCardDataDto>();
            foreach (Course course in courses)
            {
                CourseCardDataDto currentCourseCard = mapper.Map<CourseCardDataDto>(course);
                filteredCourseCards.Add(currentCourseCard);
            }
            return Ok(filteredCourseCards);
        }

        [HttpGet("GetSearchedCourses")]
        public ActionResult<List<CourseCardDataDto>> GetSearchedCourses([FromQuery] string searchString)
        {
            List<Course> courses = UnitOfWork.CourseRepository.SearchCourses(searchString, null).ToList();
            List<CourseCardDataDto> searchCourseCards = new List<CourseCardDataDto>();
            foreach (Course course in courses)
            {
                CourseCardDataDto currentCourseCard = mapper.Map<CourseCardDataDto>(course);
                searchCourseCards.Add(currentCourseCard);
            }
            return Ok(searchCourseCards);
        }

        [HttpGet("GetCertificate/{id:int}")]

        public async Task<IActionResult> GetCertificate(int id)
        {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var std = UnitOfWork.StudentRepository.Get(c => c.UserId == UserID);
                var name = std.Fname + std.Lname;
                var course = UnitOfWork.CourseRepository.Get(c => c.Id == id);
                var cert = UnitOfWork.CourseRepository.getCrsCertificate(course, name);
                return Ok(cert);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteCourse")]
        public IActionResult DeleteCourse(int courseId)
        {
            if (ModelState.IsValid)
            {
                var course = UnitOfWork.CourseRepository.Get(c => c.Id == courseId);
                course.IsDeleted = true;
                UnitOfWork.CourseRepository.remove(course);
                UnitOfWork.save();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetNotApprovedCourses")]
        public ActionResult<List<NotApprovedCoursesDTO>> GetNotApprovedCourses()
        {
            List<Course> courses = UnitOfWork.CourseRepository.GetNotApprovedCourses().ToList();
            List<NotApprovedCoursesDTO> courseNotApproved = new List<NotApprovedCoursesDTO>();
            foreach (Course course in courses)
            {
                NotApprovedCoursesDTO currentCourse = mapper.Map<NotApprovedCoursesDTO>(course);
                courseNotApproved.Add(currentCourse);
            }
            return Ok(courseNotApproved);
        }
        [HttpPut("ApproveCourse")]
        public IActionResult ApproveCourse(int courseId)
        {
            if (ModelState.IsValid)
            {
                var course = UnitOfWork.CourseRepository.Get(c => c.Id == courseId);
                if (course == null)
                {
                    return NotFound("Course not found");
                }
                course.IsApproved = true;
                UnitOfWork.CourseRepository.update(course);
                UnitOfWork.save();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetTotalNumOfCourse")]
        public IActionResult GetTotalNumOfCourse()
        {
            int numofcourse = UnitOfWork.CourseRepository.GetTotalNumOfCourse();
            return Ok(numofcourse);
        }

        [HttpGet("GetLastThreeCourses")]
        public IActionResult GetLastThreeCourses()
        {
            List<Course> newcourses = UnitOfWork.CourseRepository.GetLastThreeCourses();
            return Ok(newcourses);
        }
        [HttpGet("GetTopEarningCourses")]
        public IActionResult GetTopEarningCourses()
        {
            return Ok(UnitOfWork.CourseRepository.GetTop3CoursesByEarnings());
        }
        [HttpGet("GetNumOfStdsOfCourse/{courseId:int}")]
        public IActionResult GetNumOfStdsOfCourseById(int courseId){
            return Ok(UnitOfWork.StudentEnrolledCoursesRepository.GetNumOfStdsOfCourseById(courseId));
        }
        

    }
}
