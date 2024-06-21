using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class EarningsRepository : Repository<Earnings>, IEarningsRepository
    {
        private readonly ApplicationDbContext Context;
        public EarningsRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }
        public decimal GetTotalInstructorEarnings()
        {
            var totalInstructorEarnings = Context.Earnings
                .Where(e => e.enrollment != null && e.enrollment.Course != null)
                .Sum(e => e.InstructorEarnings);

            return totalInstructorEarnings;
        }
        public decimal GetTotalWebsiteEarnings()
        {
            var totalWebsiteEarnings = Context.Earnings
                .Where(e => e.enrollment != null && e.enrollment.Course != null)
                .Sum(e => e.WebsiteEarnings);

            return totalWebsiteEarnings;
        }
    }
}
