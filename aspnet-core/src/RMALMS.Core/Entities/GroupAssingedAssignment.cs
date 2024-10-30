using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class GroupAssingedAssignment : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }
        public Guid CourseGroupId { get; set; }
        [ForeignKey(nameof(AssignmentSettingId))]
        public AssignmentSetting CourseAssignment { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public int? TenantId { get; set; }
    }
}
