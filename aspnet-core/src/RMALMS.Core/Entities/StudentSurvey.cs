using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMALMS.Entities
{
    public class StudentSurvey : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }        
        [ForeignKey(nameof(QuizSettingId))]
        public QuizSetting QuizSetting { get; set; }
        public Guid QuizSettingId { get; set; }

        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
        public TestAttemptStatus Status { get; set; }
    }
}
