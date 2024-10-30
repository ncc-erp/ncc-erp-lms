using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public class Annoucement : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int? TenantId { get; set; }
        public Guid CourseInstanceId { get; set; }
        public CourseInstance CourseInstance { get; set; }
    }
}
