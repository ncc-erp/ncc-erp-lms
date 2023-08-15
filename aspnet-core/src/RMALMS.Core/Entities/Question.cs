using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Question : FullAuditedEntity<Guid>, IMayHaveTenant, IMayHaveVersion
    {
        public int? TenantId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        //public float? Mark { get; set; }
        public int? NWord { get; set; } // set for Fixed Words
        public QuestionType Type { get; set; }
        public QuestionCate Group { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        public string Version { get; set; }
        //public int Index { get; set; }
    }
    public enum QuestionType : byte
    {
        MCQ = 0, // Multiple Choice Questions
        SCQ = 1, // Single Choice Questions
        OpenEnd = 2, // Open-End
        FixedWord = 3, // Fixed Word
        Ranked = 4,
        Matching = 5,
        TrueFalse = 6,
        MatrixTableQuestion = 7
    }

    public enum QuestionCate: int
    {
        Quiz = 0,
        Feedback = 1
    }
}
