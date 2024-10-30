using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentAssingedAssignment : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(StudentId))]
        public User Student { get; set; }
        public long StudentId { get; set; }
        [ForeignKey(nameof(AssignmentSettingId))]
        public AssignmentSetting CourseAssignment { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public int? TenantId { get; set; }
    }
}
