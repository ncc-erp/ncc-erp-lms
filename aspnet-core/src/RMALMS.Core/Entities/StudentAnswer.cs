using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentAnswer : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; }
        public Guid? QuestionId { get; set; }
        [ForeignKey(nameof(AnswerId))]
        public Answer Answer { get; set; }
        public Guid? AnswerId { get; set; }
        public string AnswerText { get; set; }
        public float? Mark { get; set; }
        public string QuestionVersion { get; set; }
        [ForeignKey(nameof(TestAttempId))]
        public TestAttempt TestAttempt { get; set; }
        public Guid? TestAttempId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; }
        public Guid? ModuleId { get; set; }
        public StudentAnswerStatus Status { get; set; }
    }

    public enum StudentAnswerStatus : byte
    {
        JustInitial = 0,
        StudentAnswer = 1
    }

}
