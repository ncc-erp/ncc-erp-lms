using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentProgressScorm : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string PageId { get; set; }//identifier
        public StudentProgressStatus Progress { get; set; }
        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
    }    
}
