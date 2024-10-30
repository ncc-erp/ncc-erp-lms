using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class PageLinkExam : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid PageId { get; set; }
        [ForeignKey(nameof(PageId))]
        public Page Page { get; set; }
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public int? SequenceOrder { get; set; }
    }
}
