using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class QuestionQuiz : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; }
        [ForeignKey(nameof(QuizId))]
        public Quiz Quiz { get; set; }
        public Guid QuestionId { get; set; }
        public Guid QuizId { get; set; }
        public int? TenantId { get; set; }
        public float? Mark { get; set; }
        public int Index { get; set; }
    }
}
