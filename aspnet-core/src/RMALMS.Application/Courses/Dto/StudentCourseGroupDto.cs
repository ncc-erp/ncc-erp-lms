using Abp.Application.Services.Dto;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class StudentCourseGroupDto
    {
        public List<Guid> AssignedStudentIds { get; set; }
        public Guid CourseGroupId { get; set; }
    }

    public class StudentCourseGroupListDto : EntityDto<Guid>
    {
        public string StudentName { get; set; }
        public Guid AssignedStudentId { get; set; }
        public int EnrollCount { get; set; }
        public AssignedStatus Status { get; set; }
    }

 
}
