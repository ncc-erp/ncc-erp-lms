using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class ScormTestAttempt : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public TestAttemptStatus Status { get; set; }        
        public float Score { get; set; }
        public float MaxScore { get; set; }
        public string Name { get; set; }
        public bool IsFinal { get; set; }
        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
    }   
}
