using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Quizzes.Dto
{
    [AutoMapTo(typeof(Quiz))]
    public class SelectQuizDto: EntityDto<Guid>
    {
        public string Title { get; set; }
        public QuizType Type { get; set; }
    }
}
