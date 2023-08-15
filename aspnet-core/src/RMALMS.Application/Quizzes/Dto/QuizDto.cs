using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RMALMS.Quizzes.Dto
{
    public class QuizDto : EntityDto<Guid>
    {
        [Required]
        [ApplySearchAttribute]
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
        //public float? Point { get; set; }
        public Guid CourseInstanceId { get; set; }
        public QuizSettingsDto settings { get; set; }
        public List<Guid> GroupsAssingedQuiz { get; set; }
        public bool AllowNotify { get; set; }

    }
}
