using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;
using System;
using System.Collections.Generic;

namespace RMALMS.QAQuestions.Dto
{
    #region FAQ Question
    public class FAQQuestionDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int SequenceOrder { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class FAQQnADto : FAQQuestionDto
    {
        public IEnumerable<FAQAnswerDto> FAQAnswers { get; set; }
    }

    public class FAQQuestionInput
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CourseId { get; set; }
    }

    #endregion

    #region FAQ Answer
    public class FAQAnswerDto : FAQAnswerInput
    {
        public Guid Id { get; set; }
        public int SequenceOrder { get; set; }
    }

    public class FAQAnswerInput
    {
        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        public string ReplyUserName { get; set; }
    }
    #endregion

    public class FAQSequenceOrderInput
    {
        public Guid Id { get; set; }
        public int SequenceOrder { get; set; }
    }



}
