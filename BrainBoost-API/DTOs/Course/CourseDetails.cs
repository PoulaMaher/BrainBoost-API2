using BrainBoost_API.DTOs.Review;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainBoost_API.DTOs.Course
{
    public class CourseDetails
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? photoUrl { get; set; }
        public int? Rate { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
       public List<ReviewDTO>? Review { get; set; }
        public List<WhatToLearnDTO>? WhatToLearn { get; set; }


    }
}
