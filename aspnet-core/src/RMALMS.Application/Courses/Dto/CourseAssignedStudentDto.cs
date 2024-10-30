using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class CourseAssignedStudentDto : EntityDto<Guid>
    {
        public long StudentId { get; set; }
        public AssignedStatus Status { get; set; }

        public Guid CourseInstanceId { get; set; }
    }

    public class CreateUpdateCourseAssignedStudentDto : EntityDto<Guid>
    {
        public long StudentId { get; set; }
        [JsonIgnore]
        public AssignedStatus Status { get; set; }
        [JsonIgnore]
        public AssignedSource AssignedSource { get; set; }

        public Guid CourseInstanceId { get; set; }
    }

    public class StudentEnrollDto
    {
        //public long StudentId { get; set; }
        public Guid CourseInstanceId { get; set; }
    }

    public class StudentAcceptRejectDto
    {
        //public long StudentId { get; set; }
        public Guid CourseInstanceId { get; set; }
        public AssignedStatus Status { get; set; }
    }

    public class UpdateCourseAssignedStudentStatusDto : EntityDto<Guid>
    {        
        public AssignedStatus Status { get; set; }
        public string UserName { get; set; }
        
    }
}
