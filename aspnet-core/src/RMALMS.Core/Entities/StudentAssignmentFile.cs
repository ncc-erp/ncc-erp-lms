using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentAssignmentFile : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(AssignmentSettingId))]
        public AssignmentSetting Assignment { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public string FileName { get; set; }
        public Guid? CourseGroupId { get; set; }
        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }

        public string FilePath { get; set; }
        public string MineType { get; set; }

        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
    }
}
