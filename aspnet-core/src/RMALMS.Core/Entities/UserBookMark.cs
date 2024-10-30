using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class UserBookMark : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid PageId { get; set; }
        [ForeignKey(nameof(PageId))]
        public Page Page { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
}
