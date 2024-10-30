using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentScorm : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
    
}
