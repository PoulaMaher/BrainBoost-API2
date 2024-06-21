using AutoMapper;
using BrainBoost_API.DTOs.Course;
using BrainBoost_API.DTOs.Review;
using BrainBoost_API.DTOs.Teacher;
using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext Context;
        private readonly IMapper mapper;

        public CourseRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            this.Context = context;
            this.mapper = mapper;
        }

        public CourseDetailsDto getCrsDetails(Course crs, List<Review> review)
        {
            if (crs != null)
            {
                CourseDetailsDto crsDetails = mapper.Map<CourseDetailsDto>(crs);

                crsDetails.Review = mapper.Map<IEnumerable<ReviewDTO>>(review).ToList();
                crsDetails.WhatToLearn = mapper.Map<IEnumerable<WhatToLearnDTO>>(crs.WhatToLearn).ToList();
                crsDetails.TeacherDataDto = mapper.Map<CourseDetailsTeacherDataDto>(crs.Teacher);



                return crsDetails;
            }
            return new CourseDetailsDto();
        }
        public IEnumerable<Course> GetFilteredCourses(CourseFilterationDto filter, string? includeProps = null)
        {
            IQueryable<Course> courses = GetAll(includeProps).AsQueryable();
            if (!string.IsNullOrEmpty(filter.CategoryName))
            {
                courses = courses.Where(c => c.Category.Name == filter.CategoryName);
            }
            if (filter.Price != -1)
            {
                if (filter.Price == 0)
                {
                    courses = courses.Where(c => c.Price == filter.Price);
                }
                if (filter.Price > 0)
                {
                    courses = courses.Where(c => c.Price >= filter.Price);
                }
            }
            if (filter.Rate != -1)
            {
                courses = courses.Where(c => c.Rate == filter.Rate);
            }
            var filteredCourses = new List<Course>();
            filteredCourses = courses.ToList();
            return filteredCourses;
        }
        public List<Course> SearchCourses(string searchString, string? includeProps)
        {
            var courses = GetList(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)
                            || c.Teacher.Fname.Contains(searchString) || c.Teacher.Lname.Contains(searchString), includeProps);


            if (courses != null)
            {
                return courses.ToList();
            }
            else
            {
                return new List<Course>();
            }
        }
        public CertificateDTO getCrsCertificate(Course crs, string s)
        {
            if (crs != null)
            {
                var cert = mapper.Map<CertificateDTO>(crs);
                cert.StdName = s;
                return cert;

            }
            return new CertificateDTO();

        }
        public IEnumerable<Course> GetNotApprovedCourses(string? includeProps = null)
        {
            IQueryable<Course> courses = GetAll(includeProps).AsQueryable();
            courses = courses.Where(c => c.IsApproved == false);
            var filteredCourses = new List<Course>();
            filteredCourses = courses.ToList();
            return filteredCourses;
        }

        public int GetTotalNumOfCourse()
        {
            int numofCourse = Context.Courses.Count<Course>();
            return numofCourse;
        }

        public List<Course> GetLastThreeCourses()
        {
            List<Course> lastThreeCourses = Context.Courses
                                            .OrderByDescending(c => c.LastUpdate)
                                            .Take(3)
                                            .ToList();
            return lastThreeCourses;
        }
        public List<CourseEarningsDto> GetTop3CoursesByEarnings()
        {
            var topCourses = Context.Earnings
                .Where(e => e.enrollment != null && e.enrollment.Course != null)
                .GroupBy(e => e.enrollment.Course)
                .OrderByDescending(g => g.Sum(e => e.Amount))
                .Take(3)
                .Select(g => new
                {
                    Course = g.Key,
                    TotalEarnings = g.Sum(e => e.Amount),
                    TotalInstructorEarnings = g.Sum(e => e.InstructorEarnings),
                    TotalWebsiteEarnings = g.Sum(e => e.WebsiteEarnings)
                })
                .ToList()
                .Select(x =>
                {
                    var dto = mapper.Map<CourseEarningsDto>(x.Course);
                    dto.TotalEarnings = x.TotalEarnings;
                    dto.TotalInstructorEarnings = x.TotalInstructorEarnings;
                    dto.TotalWebsiteEarnings = x.TotalWebsiteEarnings;
                    return dto;
                })
                .ToList();

            return topCourses;
        }
    }
}
