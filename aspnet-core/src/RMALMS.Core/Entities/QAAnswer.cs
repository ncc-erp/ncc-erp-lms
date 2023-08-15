using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class QAAnswer : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public QAQuestion Question { get; set; }
        public Guid? ResponseParentId { get; set; }
        [ForeignKey(nameof(ResponseParentId))]
        public QAAnswer Answer { get; set; }
    }
}
