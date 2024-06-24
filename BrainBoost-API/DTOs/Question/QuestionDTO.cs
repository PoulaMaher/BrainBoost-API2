using BrainBoost_API.DTOs.Answer;

namespace BrainBoost_API.DTOs.Question
{
    public class QuestionDTO
    {
        public string HeadLine { get; set; }
        public int Degree { get; set; }
        public List<AnswerDTO> Choices { get; set; }
    }
}
