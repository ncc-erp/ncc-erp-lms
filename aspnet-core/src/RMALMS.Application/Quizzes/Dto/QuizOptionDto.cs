using Abp.Application.Services.Dto;
using RMALMS.Entities;
using RMALMS.TestAttempts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Quizzes.Dto
{
    public class QuizOptionDto: EntityDto<Guid>
    {
        public bool IsShuffleAnswer { get; set; }
        public int? TimeLimit { get; set; }
        public int? AllowAttempts { get; set; }
        public QuizScoreToKeepType ScoreKeepType { get; set; }
        public bool ShowOneQuestionAtATime { get; set; }
        public bool LookQuestionAfterAnswer { get; set; }
        public StudentReponseType ResponseType { get; set; }
        //public float? Point { get; set; }
        public QuizSettingsDto settings { get; set; }

        public string Content { get; set; }

        public List<TestAttemptDto> TestAttempts { get; set; }
        public TestAttemptDto TestingAttempt { get; set; }
        public bool IsExpired { get; set; }

    }
}
