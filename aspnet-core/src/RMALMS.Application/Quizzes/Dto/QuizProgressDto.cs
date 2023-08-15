using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Quizzes.Dto
{
    [AutoMapTo(typeof(Quiz))]
    public class QuizProgressDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public float? QuizScore { get; set; }
        public float? StudentScore { get; set; }
        public Guid QuizSettingId { get; set; }
        public QuizScoreToKeepType ScoreType { get; set; }
    }
}
