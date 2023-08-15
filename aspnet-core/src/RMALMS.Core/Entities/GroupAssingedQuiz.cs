using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class GroupAssingedQuiz : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }
        public Guid CourseGroupId { get; set; }
        [ForeignKey(nameof(QuizSettingId))]
        public QuizSetting QuizSetting { get; set; }
        public Guid QuizSettingId { get; set; }
        public int? TenantId { get; set; }
    }
}
