using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseInstance : FullAuditedEntity<Guid>, IMayHaveTenant, IMayHaveVersion
    {
        public bool AllowSkip { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public float PassingMark { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        public int? TenantId { get; set; }
        public int TotalQuiz { get; set; }
        public string Version { get; set; }
        public bool AllowFinalQuizRetry { get; set; }
        public int? NumberDayToStudy { get; set; }
        public CourseSettingStatus Status { get; set; }
        public bool EnableCourseGradingScheme { get; set; }
    }

    public enum CourseSettingStatus : byte
    {
        Active = 0,
        Deactive = 1
    }
}
