using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Questions.Dto
{
    [AutoMapTo(typeof(StudentAnswer))]
    public class StudentAnswerDto : EntityDto<Guid>
    {
        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }
        public string AnswerText { get; set; }
        public Guid? TestAttempId { get; set; }
        public float? Mark { get; set; }
    }

    public class TeacherStudentAnswerDto
    {        
        public Guid QuestionQuizId { get; set; }
        public Guid TestAttempId { get; set; }
        //public Guid  CourseAssignedStudentId { get; set; }
        public float? Mark { get; set; }
    }

    public class StudentAnswerQuestionDto
    {
        public Guid QuestionId { get; set; }
        public Guid TestAttempId { get; set; }

        public  List<StudentAnswerDto> StudentAsnwers { get; set; }
    }
}
