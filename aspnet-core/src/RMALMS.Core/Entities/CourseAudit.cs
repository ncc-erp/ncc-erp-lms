using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseAudit : CreationAuditedEntity<Guid>, IMayHaveTenant, IMayHaveVersion
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(StatusId))]
        public CourseLevel Status { get; set; }
        public Guid? StatusId { get; set; }
        public string ImageCover { get; set; }
        public CourseType Type { get; set; }
        public string Version { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        public int? TenantId { get; set; }
    }
}
