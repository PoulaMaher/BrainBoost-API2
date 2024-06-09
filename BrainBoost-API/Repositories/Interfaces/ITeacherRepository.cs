using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;

namespace BrainBoost_API.Repositories.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Teacher GetTeacherById(int id);
        List<Course> GetCoursesForTeacher(int TeacherId);
    }
}
