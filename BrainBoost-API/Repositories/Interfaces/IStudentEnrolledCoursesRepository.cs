﻿using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public interface IStudentEnrolledCoursesRepository : IRepository<StudentEnrolledCourses>
    {
        int GetNumOfStdsOfCourseById(int courseId);
    }
}
