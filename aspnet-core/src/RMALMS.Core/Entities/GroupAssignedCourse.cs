using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class GroupAssignedCourse : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public Guid CourseInstanceId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }
        public Guid GroupId { get; set; }
    }
}
