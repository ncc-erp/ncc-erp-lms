using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentAssignment : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public float? Point { get; set; }
        [ForeignKey(nameof(AssignmentSettingId))]
        public AssignmentSetting Assignment { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
    }
}
