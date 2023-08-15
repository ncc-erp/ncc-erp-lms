using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseAssignedStudent : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public User Student { get; set; }
        public long StudentId { get; set; }
        public AssignedStatus Status { get; set; }

        public Guid CourseInstanceId { get; set; }
        [ForeignKey(nameof(CourseInstanceId))]
        public CourseInstance CourseInstance { get; set; }
        public AssignedSource AssignedSource { get; set; }

        public int EnrollCount { get; set; }
    }

    public enum AssignedStatus : byte
    {
        Invited = 0,//table 2 
        PendingApproved = 1, // for enroll, re-enroll
        Accepted = 2,//table 1 only
        Rejected = 3,
        Completed = 4,
    }

    public enum AssignedSource: byte
    {
        Direct = 0,
        FromEnroll = 1
    }
}
