using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class GroupAssignedCourseDto : EntityDto<Guid>
    {
        public Guid GroupId { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
}
