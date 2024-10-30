using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Questions.Dto
{
    //[AutoMapTo(typeof(Question))]
    public class CreateQuestionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public float? Mark { get; set; }
        public int? NWord { get; set; }
        public QuestionType Type { get; set; }
        public QuestionCate Group { get; set; }
        public Guid CourseId { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid QuizId { get; set; }//

        public CreateAnswerDto[] Answers { get; set; }
    }
}
