using AutoMapper;
using RMALMS.Annoucements.Dto;
using RMALMS.Assignments.Dto;
using RMALMS.Authorization.Accounts.Dto;
using RMALMS.Authorization.Users;
using RMALMS.Courses.Dto;
using RMALMS.CourseSettings.Dto;
using RMALMS.Entities;
using RMALMS.GradeSchemes.Dto;
using RMALMS.Groups.Dto;
using RMALMS.Questions.Dto;
using RMALMS.Quizzes.Dto;
using RMALMS.TimeZone.Dto;
using RMALMS.Pages.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.TestAttempts.Dto;
using RMALMS.CertificationTemplates.Dto;

namespace RMALMS.AutoMapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            // This is default auto mapper
            //CreateMap<Group, GroupDto>()
            //    .ForMember(s => s.ParentName, opt => opt.MapFrom(s => s.Parent.Name))
            //    .ReverseMap()
            //    .ForMember(s => s.Parent, opt => opt.Ignore());

            CreateMap<Course, EditCourseDto>(MemberList.None)
                .ReverseMap();
            CreateMap<Question, CreateQuestionDto>(MemberList.None)
                .ForMember(s => s.Answers, opt => opt.Ignore())
                .ForMember(s => s.Mark, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Question, QuestionDto>(MemberList.None)
                .ForMember(s => s.Answers, opt => opt.Ignore())
                .ForMember(s => s.Mark, opt => opt.Ignore())
                 .ForMember(s => s.QuestionQuizId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CourseGroup, CourseGroupDto>(MemberList.None)
                .ReverseMap();

            CreateMap<User, UserInfoDto>(MemberList.None)
                .ReverseMap()
                .ForMember(s => s.Status, opt => opt.Ignore());

            CreateMap<CourseInstance, CourseInstanceDto>(MemberList.None)
                .ReverseMap();

            CreateMap<GroupAssignedCourse, GroupAssignedCourseDto>(MemberList.None)
                .ReverseMap();

            CreateMap<CourseAssignedStudent, CourseAssignedStudentDto>(MemberList.None)
                .ReverseMap();

            CreateMap<CreateUpdateCourseAssignedStudentDto, CourseAssignedStudent>(MemberList.None)
                .ReverseMap();

            CreateMap<UserLink, UserLinkDto>(MemberList.None)
                .ReverseMap();

            CreateMap<UserTimeZone, UserTimeZoneDto>(MemberList.None);

            CreateMap<Quiz, QuizDto>(MemberList.None)
                .ForMember(s => s.settings, opt => opt.Ignore())
                .ForMember(s => s.CourseInstanceId, opt => opt.Ignore())
                .ForMember(s => s.GroupsAssingedQuiz, opt => opt.Ignore())
                .ForMember(s => s.AllowNotify, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Assignment, AssignmentDto>(MemberList.None)
               .ForMember(s => s.settings, opt => opt.Ignore())
               .ForMember(s => s.CourseInstanceId, opt => opt.Ignore())
               .ForMember(s => s.GroupsAssingedAssignment, opt => opt.Ignore())
               .ForMember(s => s.IsDisable, opt => opt.Ignore())
               .ForMember(s => s.AllowNotify, opt => opt.Ignore())
               .ReverseMap();
            CreateMap<GradeScheme, GradeSchemeDto>(MemberList.None)
             .ForMember(s => s.ElementList, opt => opt.Ignore())
             .ReverseMap();
            CreateMap<GradeScheme, CreateGradeSchemeDto>(MemberList.None)
               .ForMember(s => s.ElementList, opt => opt.Ignore())
               .ReverseMap();
            CreateMap<Annoucement, AnnoucementDto>(MemberList.None)
             .ForMember(s => s.ImageCover, opt => opt.Ignore())
             .ForMember(s => s.UserName, opt => opt.Ignore())
             .ReverseMap();

            CreateMap<Page, PageDto>(MemberList.None)
                .ForMember(s => s.Links, opt => opt.Ignore())
                .ForMember(s => s.Bookmarked, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Page, CreatePageDto>(MemberList.None)
                .ForMember(s => s.Links, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<StudentAnswer, StudentAnswerDto>(MemberList.None)
                .ReverseMap();

            CreateMap<Quiz, QuizOptionDto>(MemberList.None)
                .ForMember(s => s.TestAttempts, opt => opt.Ignore())
                .ForMember(s => s.TestingAttempt, opt => opt.Ignore())
                .ForMember(s => s.IsExpired, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<TestAttempt, TestAttemptDto>(MemberList.None)
                .ForMember(s => s.TimeRemaining, opt => opt.Ignore())
                .ForMember(s => s.StudentAnswers, opt => opt.Ignore())
                .ForMember(s => s.Questions, opt => opt.Ignore())
                .ForMember(s => s.Type, opt => opt.Ignore())
                .ForMember(s => s.UserCertification, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CourseCertificationTemplate, CertificationTemplateDto>(MemberList.None)
                   .ForMember(s => s.File, opt => opt.Ignore())
                   .ReverseMap();
            CreateMap<StudentAssignment, StudentAssignmentDto>(MemberList.None)
                   .ForMember(s => s.IsApplyForAllStudentInGroup, opt => opt.Ignore())
                   .ReverseMap();            
        }
    }
}
