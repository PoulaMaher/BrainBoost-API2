﻿using AutoMapper;
using BrainBoost_API.DTOs.Answer;
using BrainBoost_API.DTOs.Course;
using BrainBoost_API.DTOs.Enrollment;
using BrainBoost_API.DTOs.Question;
using BrainBoost_API.DTOs.Quiz;
using BrainBoost_API.DTOs.Review;
using BrainBoost_API.DTOs.Student;
using BrainBoost_API.DTOs.Subscription;
using BrainBoost_API.DTOs.Teacher;
using BrainBoost_API.Models;

namespace BrainBoost_API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SubscriptionDto, subscription>().ReverseMap();
            CreateMap<CourseDetailsDto, Course>().ReverseMap();
            CreateMap<ReviewDTO, Review>().ReverseMap();
            CreateMap<WhatToLearnDTO, WhatToLearn>().ReverseMap();
            CreateMap<EnrollmentDto, Enrollment>().ReverseMap();
            CreateMap<QuizDTO, Quiz>().ReverseMap();
            CreateMap<DTOs.Question.QuestionDTO, Question>().ReverseMap();
            CreateMap<DTOs.Answer.AnswerDTO, Answer>().ReverseMap();
            CreateMap<CertificateDTO, Course>().ReverseMap();
            CreateMap<Course, CourseCardDataDto>();
            CreateMap<Course, NotApprovedCoursesDTO>().ReverseMap();
            CreateMap<Teacher, CourseCardTeacherDataDto>();
            CreateMap<Teacher, TeacherDataDTO>().ReverseMap();
            CreateMap<CourseDetailsTeacherDataDto, Teacher>().ReverseMap();
            CreateMap<StudentDTO, Student>().ReverseMap();


        }
    }
}
