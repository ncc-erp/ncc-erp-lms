using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Questions.Dto
{    
   public class QuestionDto: EntityDto<Guid>
    {
        [Required]
        [ApplySearchAttribute]
        public string Title { get; set; }
        public string Description { get; set; }
        public float? Mark { get; set; }
        public int? NWord { get; set; }
        public QuestionType Type { get; set; }
        public QuestionCate Group { get; set; }
        public Guid CourseId { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid QmqId { get; set; }
        public List<AnswerDto> Answers { get; set; }

        public Guid QuestionQuizId { get; set; }
        public int? Index { get; set; }
    }
}
