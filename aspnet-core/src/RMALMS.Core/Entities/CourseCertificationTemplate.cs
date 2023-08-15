using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseCertificationTemplate : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public bool IsActive { get; set; }
        public string Background { get; set; }
        public string Content { get; set; }
        public CertificationType CertificationType { get; set; }
        public string Name { get; set; }
        public CertificationOrientation Orientation { get; set; }
    }

    public enum CertificationType : byte
    {
        Attendance = 0,
        Completion = 1
    }

    public enum CertificationOrientation : byte
    {
        Landscape = 0,
        Portrait = 1
    }
}
