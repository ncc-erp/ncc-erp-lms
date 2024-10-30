using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class AssignmentSetting : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid CourseInstanceId { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public int? NumberOfDueDays { get; set; }
        public Guid AssingmentId { get; set; }
        [ForeignKey(nameof(AssingmentId))]
        public Assignment Assignment { get; set; }
        public float? Point { get; set; }
    }

    public enum AssignmentSettingStatus
    {
        Active = 0,
        InActive = 1
    }
}
