namespace BrainBoost_API.DTOs.Teacher
{
    public class TeacherDataDTO
    {
        public int Id { get; set; }
        public int NumOfCrs { get; set; }
        public int NumOfFollowers { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? PictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Career { get; set; }
    }
}
