using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public interface IEarningsRepository : IRepository<Earnings>
    {
        decimal GetTotalInstructorEarnings();
        decimal GetTotalWebsiteEarnings();
        decimal GetTotalEarning();
    }
}
