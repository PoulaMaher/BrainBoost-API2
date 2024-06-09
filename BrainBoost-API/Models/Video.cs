using System.ComponentModel.DataAnnotations.Schema;

namespace BrainBoost_API.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsDeleted { get; set; } = false;


        [ForeignKey("Course")]
        public int CrsId { get; set; }
        

        public Course? Course { get; set; }
    }
}
