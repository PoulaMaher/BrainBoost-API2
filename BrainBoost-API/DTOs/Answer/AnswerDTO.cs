using System.ComponentModel.DataAnnotations.Schema;

namespace BrainBoost_API.DTOs.Answer
{
    public class AnswerDTO
    {
        public string Content { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
