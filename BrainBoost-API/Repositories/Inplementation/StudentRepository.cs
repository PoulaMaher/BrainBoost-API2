using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Interfaces;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class StudentRepository : Repository<Student> , IStudentRepository
    {
        private readonly ApplicationDbContext Context;
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }
    }
}
