namespace BrainBoost_API.Services.Uploader
{
    public class Uploader
    {
        public static async Task<string> uploadPhoto(IFormFile InsertedPhoto, string WhereToStore, string folderName)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Images\\{WhereToStore}\\{folderName}");
            string photoUrl = "";
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            var filePath = Path.Combine(uploads, InsertedPhoto.FileName);
            var fileStream = new FileStream(filePath, FileMode.Create);
            using (fileStream)
            {
                await InsertedPhoto.CopyToAsync(fileStream);
            }
            photoUrl = $"http://localhost:43827/Images/{WhereToStore}/{folderName}/{InsertedPhoto.FileName}";
            return photoUrl;
        }
    }
}
