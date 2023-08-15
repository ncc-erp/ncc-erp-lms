using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Questions.Dto
{
    [AutoMapTo(typeof(Answer))]
    public class AnswerDto: EntityDto<Guid>
    {
        [Required]
        public string RAnswer { get; set; }        
        public string LAnswer { get; set; }
        public bool? IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
        public int? SequenceOrder { get; set; }
    }
}
