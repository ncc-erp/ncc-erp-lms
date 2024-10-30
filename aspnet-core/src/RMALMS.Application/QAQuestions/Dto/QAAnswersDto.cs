using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace RMALMS.QAQuestions.Dto
{
    public class QAAnswersDto : UserCreaterDto
    {
        public int? TenantId { get; set; }
        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        public IEnumerable<QAAnswersDto> Answers { get; set; }
        public Guid? PId { get; set; }
        public int NumberAnswer { get; set; }
    }

    public class IdContentInput
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }
    public class QAAnswerInput
    {
        public Guid QuestionId { get; set; } // Update: Id as QuestionId
        public string Content { get; set; }
        public Guid? ResponseParentId { get; set; }
        public string ReplyUserName {get;set;}
    }
}
