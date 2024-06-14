using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class EarningsRepository : Repository<Earnings> , IEarningsRepository
    {
        private readonly ApplicationDbContext Context;
        public EarningsRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }
    }
}
