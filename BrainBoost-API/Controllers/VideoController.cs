using BrainBoost_API.DTOs.video;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public VideoController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet("GetCourseVideos/{id:int}")]
        public async Task<IActionResult> GetCrsVideo(int id)
      {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int stdId = unitOfWork.StudentRepository.Get(c => c.UserId == UserID).Id;
                var enrolledCrs = unitOfWork.StudentEnrolledCoursesRepository.Get(c => c.CourseId == id && c.StudentId == stdId);
                var videos = unitOfWork.VideoRepository.GetList(c=>c.CrsId==id).ToList();
                var videoState = unitOfWork.VideoStateRepository.GetList(c => c.StudentEnrolledCourseId == enrolledCrs.Id);
                 var videoDTO= unitOfWork.VideoStateRepository.GetVideoState(videoState, videos);
               
                return Ok(videoDTO);
            }
            return BadRequest(ModelState);
        }
        
        [HttpGet("ChangeVideoState/{id:int}")]
        public IActionResult ChangeVideoState(int id,[FromQuery] bool status)
        {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = unitOfWork.StudentRepository.Get(c => c.UserId == UserID);
                var CrsId=unitOfWork.VideoRepository.Get(c=>c.Id==id).CrsId;
                var EnrolledCrsId = unitOfWork.StudentEnrolledCoursesRepository.Get(c => c.CourseId == CrsId && c.StudentId==std.Id).Id;
                var video = unitOfWork.VideoStateRepository.Get(c => c.VideoId == id&&c.StudentEnrolledCourseId==EnrolledCrsId);
                video.State = status;
                unitOfWork.VideoStateRepository.update(video);
                unitOfWork.save();
                var videos = unitOfWork.VideoRepository.GetList(c => c.CrsId == id).ToList();
                var videoState = unitOfWork.VideoStateRepository.GetList(c => c.StudentEnrolledCourseId == EnrolledCrsId);
                var videoDTO = unitOfWork.VideoStateRepository.GetVideoState(videoState, videos);
                return Ok(videoDTO);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("ChangeAllVideosState/{id:int}")]
        public async Task<IActionResult> ChangeAllVideosState(int id, [FromQuery] bool status)
        {
            if (ModelState.IsValid)
            {
                string UserID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Student std = unitOfWork.StudentRepository.Get(c => c.UserId == UserID);

                var enrolledCourse = unitOfWork.StudentEnrolledCoursesRepository.Get(c => c.StudentId == std.Id && c.CourseId == id);
                enrolledCourse.hasFinishedallVideos = status;
                unitOfWork.StudentEnrolledCoursesRepository.update(enrolledCourse);
                unitOfWork.save();

                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}
