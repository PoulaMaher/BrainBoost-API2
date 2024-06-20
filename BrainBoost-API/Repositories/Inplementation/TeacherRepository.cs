﻿using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Interfaces;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        private readonly ApplicationDbContext Context;
        public TeacherRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }

        public Teacher GetTeacherById(int id)
        {
            Teacher teacher = Context.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return null;
            return teacher;
        }
        public List<Course> GetCoursesForTeacher(int TeacherId)
        {
            List<Course> Courses = Context.Courses
                                   .Where(c => c.TeacherId == TeacherId)
                                   .ToList();
            return Courses;

        }
        public List<Teacher> GetTopTeachers()
        {
            List<Teacher> topTeachers = Context.Teachers
                .OrderByDescending(t => t.NumOfCrs)
                .Take(6)
                .ToList();

            return topTeachers;
        }
        public int GetTotalNumOfTeachers()
        {
            int numOfTeachers = Context.Teachers.Count<Teacher>();
            return numOfTeachers;
        }
    }

}
