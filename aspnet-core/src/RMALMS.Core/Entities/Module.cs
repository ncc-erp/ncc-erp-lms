using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Module : FullAuditedEntity<Guid>, IMayHaveTenant, IMayHaveIdentifier
    {
        public int? TenantId { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        public string Description { get; set; }
        public int SequenceOrder { get; set; }
    }
}
