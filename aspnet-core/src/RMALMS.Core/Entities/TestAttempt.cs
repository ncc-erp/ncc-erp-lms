using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class TestAttempt : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public TestAttemptStatus Status { get; set; }
        [ForeignKey(nameof(QuizSettingId))]
        public QuizSetting QuizSetting { get; set; }
        public Guid QuizSettingId { get; set; }
        public float? Score { get; set; }
        public float? MaxScore { get; set; }

        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public Guid CourseAssignedStudentId { get; set; }
    }
    public enum TestAttemptStatus : byte
    {
        Open = 0,
        Testing = 1,
        Marking = 2,
        Passed = 3,
        Failed = 4
    }
}
