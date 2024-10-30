using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;
using System;
using System.Collections.Generic;

namespace RMALMS.QAQuestions.Dto
{
    [AutoMapTo(typeof(QAQuestion))]
    public class QAQuestionDto : EntityDto<Guid>
    {
        public int? TenantId { get; set; }
        //[ApplySearchAttribute]
        public string Title { get; set; }
        //[ApplySearchAttribute]
        public string Content { get; set; }
        public Guid CourseInstanceId { get; set; }
        //public CourseInstance CourseInstance { get; set; }

    }



    public class QAQuestionOut : UserCreaterDto
    {
        public int? TenantId { get; set; }
        [ApplySearchAttribute]
        public string Title { get; set; }
        [ApplySearchAttribute]
        public string Content { get; set; }
        public Guid CourseInstanceId { get; set; }
        // public IEnumerable<QAAnswersDto> QAAnswers { get; set; }
        public int Responses { get; set; }
        //public DateTime CreationTime { get; set; }
        //public bool IsFollower { get; set; }
    }

    public class DiscussionOut : UserCreaterDto
    {
        public int? TenantId { get; set; }
        [ApplySearchAttribute]
        public string Title { get; set; }
        [ApplySearchAttribute]
        public string Content { get; set; }
        public Guid CourseInstanceId { get; set; }
        public IEnumerable<QAAnswersDto> QAAnswers { get; set; }
        public int Responses { get; set; }
        public bool IsNew { get; set; }
        //public DateTime CreationTime { get; set; }
        //public bool IsFollower { get; set; }
    }

    [AutoMapTo(typeof(QAQuestion))]
    public class QAQuestionCreateInput
    {
        //[ApplySearchAttribute]
        public string Title { get; set; }
        //[ApplySearchAttribute]
        public string Content { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
    [AutoMapTo(typeof(QAQuestion))]
    public class QAQuestionUpdateInput : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
