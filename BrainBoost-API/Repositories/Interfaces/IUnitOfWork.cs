using BrainBoost_API.Repositories.Interfaces;

namespace BrainBoost_API.Repositories.Inplementation
{
    public interface IUnitOfWork
    {
        IVideoRepository VideoRepository { get; }
        IQuizRepository QuizRepository { get; }
        IStudentRepository StudentRepository { get; }
        ICourseRepository CourseRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }
        IFacebookUserRepository FacebookUserRepository { get; }
        IPlanRepository PlanRepository { get; }
        IEnrollmentRepository EnrollmentRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IAnswerRepository AnswerRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        

        IStudentEnrolledCoursesRepository StudentEnrolledCoursesRepository { get;}

        void save();

    }
}
