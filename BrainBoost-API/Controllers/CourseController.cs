﻿using AutoMapper;
using BrainBoost_API.DTOs.Course;
using BrainBoost_API.DTOs.Quiz;
using BrainBoost_API.DTOs.Uploader;
using BrainBoost_API.DTOs.Video;
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
        public async Task<IActionResult> AddCourse(CourseDTO InsertedCourse)
        {
            if (ModelState.IsValid)
            {
                Category selectedCategory = this.UnitOfWork.CategoryRepository.Get((c) => c.Name == InsertedCourse.CategoryName);
                if (ModelState.IsValid)
                {
                    Course NewCourse = new Course()
                    {
                        Name = InsertedCourse.Name,
                        Description = InsertedCourse.Description,
                        Price = InsertedCourse.Price,
                        TeacherId = InsertedCourse.TeacherId,
                        CategoryId = selectedCategory.Id,
                        Language = InsertedCourse.Language,
                        Level = InsertedCourse.Level
                    };
                    UnitOfWork.CourseRepository.add(NewCourse);
                    UnitOfWork.save();
                    foreach (var WhatToLearnPoint in InsertedCourse.WhatToLearn)
                    {
                        WhatToLearn newWhatToLearnPoint = new WhatToLearn()
                        {
                            Content = WhatToLearnPoint,
                            CrsId = NewCourse.Id
                        };
                        UnitOfWork.WhatToLearnRepository.add(newWhatToLearnPoint);
                    };
                    UnitOfWork.save();
                    Quiz newQuiz = new Quiz()
                    {
                        NumOfQuestions = InsertedCourse.Quiz.NumOfQuestions,
                        Degree = InsertedCourse.Quiz.Degree,
                        MinDegree = InsertedCourse.Quiz.Degree / 2,
                        CourseId = NewCourse.Id
                    };
                    UnitOfWork.QuizRepository.add(newQuiz);
                    UnitOfWork.save();
                    InsertedCourse.Quiz.Questions.ForEach(q =>
                    {
                        Question newQuestion = new Question()
                        {
                            Content = q.HeadLine,
                            Type = Enums.QuestionType.MultipleChoice,
                            Degree = q.Degree,
                        };
                        UnitOfWork.QuestionRepository.add(newQuestion);
                        UnitOfWork.save();
                        q.Choices.ForEach(choice =>
                        {
                            Answer answer = new Answer()
                            {
                                Content = choice.Choice,
                                QuestionId = newQuestion.Id,
                                IsCorrect = choice.isCorrect,
                            };
                            UnitOfWork.AnswerRepository.add(answer);
                            UnitOfWork.save();
                        });
                    });
                    return Ok(new { id = NewCourse.Id, WhereToStore = NewCourse.GetType().Name, FolderName = NewCourse.Name });
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("AddVideo/{courseId:int}")]
        public async Task<IActionResult> AddVideo([FromForm] VideoDTO InsertedVideo, int courseId)
        {
            Course course = UnitOfWork.CourseRepository.Get(c => c.Id == courseId);
            if (ModelState.IsValid)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{course.GetType().Name}\\{course.Name}\\chapter {InsertedVideo.Chapter}");
                string videoUrl = "";
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var filePath = Path.Combine(uploads, InsertedVideo.VideoFile.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await InsertedVideo.VideoFile.CopyToAsync(fileStream);
                }
                videoUrl = $"{Directory.GetCurrentDirectory()}/{course.GetType().Name}/{course.Name}/chapter {InsertedVideo.Chapter}/{InsertedVideo.VideoFile.FileName}";
                Video newVideo = new Video()
                {
                    Title = InsertedVideo.Title,
                    VideoUrl = videoUrl,
                    CrsId = courseId
                };
                UnitOfWork.VideoRepository.add(newVideo);
                UnitOfWork.save();
            }
            return Ok(ModelState);
        }
        [HttpPost("HandlePhoto/{courseId:int}/{WhereToStore:alpha}/{FolderName:alpha}")]
        public async Task<IActionResult> HandlePhoto(IFormFile InsertedPhoto, int courseId, string WhereToStore, string FolderName)
        {
            if (ModelState.IsValid)
            {
                //var uploads = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{WhereToStore}");
                //string photoUrl = "";
                //if (!Directory.Exists(uploads))
                //    Directory.CreateDirectory(uploads);

                //var filePath = Path.Combine(uploads, InsertedPhoto.FileName);

                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await InsertedPhoto.CopyToAsync(fileStream);
                //}
                //photoUrl = $"http://localhost:5079/Images/{InsertedPhoto.FileName}";
                //Course course = UnitOfWork.CourseRepository.Get(c => c.Id == courseId);
                //course.photoUrl = photoUrl;
                string photoUrl = "";
                photoUrl = await Uploader.uploadPhoto(InsertedPhoto, WhereToStore, FolderName);
                Course course = UnitOfWork.CourseRepository.Get(c => c.Id == courseId);
                course.photoUrl = photoUrl;
            }
            return Ok(ModelState);
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

        public IActionResult GetCertificate(int id)
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

        [HttpGet("GetTakingCourse/{id:int}")]

        public IActionResult GetTakingCourse(int id)
        {
            if (ModelState.IsValid)
            {

                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = UnitOfWork.StudentRepository.Get(c => c.UserId == UserID);
                //info of taken course
                Course TakenCourse = UnitOfWork.CourseRepository.Get(c => c.Id == id, "WhatToLearn,Teacher");
                //info of related courses
                List<Course> CoursesMayYouTake = UnitOfWork.CourseRepository.GetList(c => c.CategoryId == TakenCourse.CategoryId && c.TeacherId == TakenCourse.TeacherId).Take(3).ToList();
                //info of state
                var enrolledCourse = UnitOfWork.StudentEnrolledCoursesRepository.Get(c => c.StudentId == std.Id && c.CourseId == id);
                //mapping
                CourseTakingDTO Crs = UnitOfWork.CourseRepository.GetCourseTaking(TakenCourse, CoursesMayYouTake, enrolledCourse);

                return Ok(Crs);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetState/{id:int}")]
        public IActionResult GetState(int id)
        {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = UnitOfWork.StudentRepository.Get(c => c.UserId == UserID);

                var enrolledCourse = UnitOfWork.StudentEnrolledCoursesRepository.Get(c => c.StudentId == std.Id && c.CourseId == id);
                var state = UnitOfWork.CourseRepository.getCrsStates(enrolledCourse);

                return Ok(state);
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

        [HttpGet("GetThreeCoursesForCategory")]
        public IActionResult GetThreeCoursesForCategory()
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
        public IActionResult GetNumOfStdsOfCourseById(int courseId)
        {
            return Ok(UnitOfWork.StudentEnrolledCoursesRepository.GetNumOfStdsOfCourseById(courseId));
        }


    }
}
