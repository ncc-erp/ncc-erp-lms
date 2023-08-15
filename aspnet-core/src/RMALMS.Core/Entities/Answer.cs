using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Answer : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        [Required]
        public string RAnswer { get; set; }
        public string LAnswer { get; set; }
        public bool? IsCorrect { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
        public int? TenantId { get; set; }
        public int? SequenceOrder { get; set; }
    }
}
