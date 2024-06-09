using AutoMapper;
using BrainBoost_API.DTOs.Course;
using BrainBoost_API.DTOs.Review;
using BrainBoost_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class CourseRepository : Repository<Course> , ICourseRepository
    {
        private readonly ApplicationDbContext Context;
        private readonly IMapper mapper;

        public CourseRepository(ApplicationDbContext context,IMapper mapper) : base(context)
        {
            this.Context = context;
            this.mapper = mapper;
        }

        public CourseDetails getCrsDetails(Course crs,List<Review> review)
        {
            if(crs!=null)
            {
                CourseDetails crsDetails = mapper.Map<CourseDetails>(crs);
               
                crsDetails.Review= mapper.Map<IEnumerable<ReviewDTO>>(review).ToList();
                crsDetails.WhatToLearn= mapper.Map<IEnumerable<WhatToLearnDTO>>(crs.WhatToLearn).ToList();
                crsDetails.Fname = crs.Teacher.Fname;
                crsDetails.Lname = crs.Teacher.Lname;
                
                return crsDetails;
            }         
            return new CourseDetails();
        } public IEnumerable<Course> GetFilteredCourses(CourseFilterationDto filter , string? includeProps = null)
        {
            IQueryable<Course> courses = GetAll(includeProps).AsQueryable();
            if (!string.IsNullOrEmpty(filter.CategoryName))
            {
                courses = courses.Where(c => c.Category.Name == filter.CategoryName);
            }
            if (filter.Price != -1){
                if(filter.Price == 0)
                {
                    courses = courses.Where(c => c.Price == filter.Price);
                }
                if(filter.Price > 0)
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
         public List<Course> SearchCourses(string searchString , string? includeProps)
        {
            var courses = GetList(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)
                            || c.Teacher.Fname.Contains(searchString) || c.Teacher.Lname.Contains(searchString), includeProps);
                            

            if (courses != null)
            {
                return courses.ToList() ;
            }
            else
            {
                return new List<Course>();
            }
        }
        public CertificateDTO getCrsCertificate(Course crs ,string s)
        {
            if (crs != null) 
            {
                var cert = mapper.Map<CertificateDTO>(crs);
                cert.StdName = s;
                return cert;
                  
            }
            return new CertificateDTO();

        }
    }
}
