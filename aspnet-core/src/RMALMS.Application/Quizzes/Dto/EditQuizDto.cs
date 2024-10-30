using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Quizzes.Dto
{
    [AutoMapTo(typeof(Quiz))]
    public class EditQuizDto 
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CourseId { get; set; }
        public QuizStatus Status { get; set; }
        public QuizType Type { get; set; }
        public bool IsShuffleAnswer { get; set; }
        public int? TimeLimit { get; set; }
        public int? AllowAttempts { get; set; }
        public QuizScoreToKeepType ScoreKeepType { get; set; }
        public bool ShowOneQuestionAtATime { get; set; }
        public bool LookQuestionAfterAnswer { get; set; }
        public StudentReponseType ResponseType { get; set; }
    }
}
