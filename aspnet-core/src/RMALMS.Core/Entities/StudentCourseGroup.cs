using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentCourseGroup : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(AssignedStudentId))]
        public CourseAssignedStudent AssignedStudent { get; set; }
        public Guid AssignedStudentId { get; set; }
        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }
        public Guid CourseGroupId { get; set; }
    }
}
