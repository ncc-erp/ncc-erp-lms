using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class StudentAssingedQuiz : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(StudentId))]
        public User Student { get; set; }
        public long StudentId { get; set; }
        [ForeignKey(nameof(QuizSettingId))]
        public QuizSetting QuizSetting { get; set; }
        public Guid QuizSettingId { get; set; }
        public int? TenantId { get; set; }
    }
}
