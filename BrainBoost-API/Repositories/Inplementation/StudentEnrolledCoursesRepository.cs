using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class StudentEnrolledCoursesRepository : Repository<StudentEnrolledCourses>, IStudentEnrolledCoursesRepository
    {
        private readonly ApplicationDbContext Context;
        public StudentEnrolledCoursesRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }
    }
}
