using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Page : FullAuditedEntity<Guid>, IMayHaveTenant, IMayHaveIdentifier
    {
        public int? TenantId { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; }
        public Guid? ModuleId { get; set; }
        public int SequenceOrder { get; set; }
        public PageType Type { get; set; }
    }

    public enum PageType: byte
    {
        Page = 0,
        Quiz = 1,
        Assignment = 2,
        QuizFinal = 3,
        Survey = 4,
    }

}
