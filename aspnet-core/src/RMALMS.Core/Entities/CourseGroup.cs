using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseGroup : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public Guid CourseInstanceId { get; set; }
        public ICollection<StudentCourseGroup> StudentCourseGroups { get; set; }
        public bool IsEveryOne { get; set; }
    }
}
