using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RMALMS.Assignments.Dto
{
    [AutoMapTo(typeof(Assignment))]
    public class CreateAssignmentDto 
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public CourseAssigmentStatus Status { get; set; }
        public DisplayGradeType DisplayGrade { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public Guid CourseId { get; set; }
        //public float? Point { get; set; }
        public AssignmentSettingsDto settings { get; set; }
        public List<Guid> GroupsAssingedAssignment { get; set; }
        public bool AllowNotify { get; set; }
        public bool IsGroupAssignment { get; set; }
        public bool IsAssignIndividualGrade { get; set; }

    }
}
