using System.ComponentModel.DataAnnotations.Schema;

namespace BrainBoost_API.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public int NumOfCrs { get; set; }
        public int NumOfFollowers { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? PictureUrl { get; set; }
        [ForeignKey("AppUser")]
        public string? UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ApplicationUser ?AppUser { get; set; }
        public List<Course>? Crs { get; set; }
    }
}
