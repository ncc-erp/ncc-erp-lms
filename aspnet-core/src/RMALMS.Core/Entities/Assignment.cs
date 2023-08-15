using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Assignment : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public CourseAssigmentStatus Status { get; set; }
        public DisplayGradeType DisplayGrade { get; set; }
        public SubmissionType SubmissionType { get; set; }
        //public float? Point { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public bool IsGroupAssignment { get; set; }
        public bool IsAssignIndividualGrade { get; set; }
    }

    public enum DisplayGradeType : byte
    {
        CompleteInComplete = 0,
        Percentage = 1,
        Points = 2,
        NotGrade = 3
    }

    public enum SubmissionType : byte
    {
        NoSubmission = 0,
        Online = 1,
        OnPaper = 2
    }
    public enum CourseAssigmentStatus : byte
    {
        Draft = 0,
        Publish = 1
    }
}
