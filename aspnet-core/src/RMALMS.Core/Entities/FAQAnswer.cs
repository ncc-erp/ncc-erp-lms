using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class FAQAnswer : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public FAQQuestion Question { get; set; }
        public int SequenceOrderId { get; set; }
    }
}
