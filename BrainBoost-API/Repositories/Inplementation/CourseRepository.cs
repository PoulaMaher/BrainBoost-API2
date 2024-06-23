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

        public CourseTakingDTO GetCourseTaking(Course takingcourse, IEnumerable<Course> relatedCourses, StudentEnrolledCourses states)
        {
            var Crs= mapper.Map<CourseTakingDTO>(takingcourse);
            Crs.CourseCardData=mapper.Map<IEnumerable<CourseCardDataDto>>(relatedCourses).ToList();
            Crs.states=mapper.Map<StateDTO>(states);
            Crs.WhatToLearn = mapper.Map<IEnumerable<WhatToLearnDTO>>(takingcourse.WhatToLearn).ToList();
            Crs.TeacherDataDto = mapper.Map<CourseDetailsTeacherDataDto>(takingcourse.Teacher);


            return Crs;
        }
        public StateDTO getCrsStates(StudentEnrolledCourses states)
        {
            var States = mapper.Map<StateDTO>(states);
            return States;
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

        public List<Course> GetThreeCoursesForCategory(int categoryid)
        {
            List<Course> lastThreeCourses = Context.Courses
                                            .Where(c => c.CategoryId == categoryid)
                                            .OrderByDescending(c => c.LastUpdate)
                                            .Take(3)
                                            .ToList();
            return lastThreeCourses;
        }
    }
}
