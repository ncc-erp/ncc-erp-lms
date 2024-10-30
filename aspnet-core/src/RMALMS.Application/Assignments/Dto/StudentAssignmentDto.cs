using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Assignments.Dto
{
    //[AutoMapTo(typeof(StudentAssignment))]
    public class StudentAssignmentDto: EntityDto<Guid>
    {
        public Guid AssignmentSettingId { get; set; }
        //public long StudentId { get; set; }
        public Guid CourseAssignedStudentId { get; set; }
        public float? Point { get; set; }
        public bool IsApplyForAllStudentInGroup { get; set; }
    }
}
