using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseContent : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        public string MineType { get; set; }
        public CourseContentType Type { get; set; }
        public Guid? Source { get; set; }
        [ForeignKey(nameof(Source))]
        public MediaContent MediaContent { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public Module Module { get; set; }
        public Guid? ModuleId { get; set; }
    }

    public enum CourseContentType
    {
        Audio = 0,
        Image = 1,
        Video = 2
    }
}
