using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Quiz : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int? TenantId { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
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
    }

    public enum QuizScoreToKeepType : byte
    {
        Highest = 0,
        Avarage = 1
    }

    public enum StudentReponseType : byte
    {
        OnlyAfterLastAttempt = 0,
        OnlyOnceAfterEachAttempt = 1,
        SeeTheCorrectAnswer = 2,
        DontSeeTheCorrectAnswer = 10 //By default, students are not allow to see the answer of the question. 
    }

    public enum QuizStatus : byte
    {
        Draft = 0,
        Published = 1
    }

    public enum QuizType : byte
    {
        Quiz = 0,
        Survey = 1
    }
}
