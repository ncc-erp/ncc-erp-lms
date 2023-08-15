using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class UserCertification : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        [ForeignKey(nameof(CourseAssignedStudentId))]
        public CourseAssignedStudent CourseAssignedStudent { get; set; }
        //public long StudentId { get; set; }
        public Guid CourseAssignedStudentId { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public Guid CourseInstanceId { get; set; }
        public Guid? TemplateId { get; set; }
        public CourseCertificationTemplate Template { get; set; }
        [ForeignKey(nameof(TemplateId))]
        public string Certification { get; set; }
        public string GraduatedLevel { get; set; }
        public float Point { get; set; }
        public float TotalPoint { get; set; }
    }
}
