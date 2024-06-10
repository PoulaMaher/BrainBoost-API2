using BrainBoost_API.Models;

namespace BrainBoost_API.Repositories.Inplementation
{
    public class VideoStateRepository : Repository<VideoState> , IVideoStateRepository
    {
        private readonly ApplicationDbContext Context;
        public VideoStateRepository(ApplicationDbContext context) : base(context)
        {
            this.Context = context;
        }
    }
}
