using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class QuestionAudit : CreationAuditedEntity<Guid>, IMayHaveTenant, IMayHaveVersion
    {
        public string Version { get; set; }
        public int? TenantId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public float? Mark { get; set; }
        public int? NWord { get; set; } // set for Fixed Words
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; }
        public Guid? QuestionId { get; set; }
        [ForeignKey(nameof(CourseAuditId))]
        public CourseAudit CourseAudit { get; set; }
        public Guid? CourseAuditId { get; set; }
    }
}
