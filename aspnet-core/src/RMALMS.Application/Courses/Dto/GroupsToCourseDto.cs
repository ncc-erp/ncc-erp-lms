using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class GroupsToCourseDto
    {
        public Guid CourseInstanceId { get; set; }
        public IEnumerable<Guid> Groups { get; set; }
    }

    public class BrifGroupDto: EntityDto<Guid>
    {
        public string Name { get; set; }

    }

    public class StudentsToCourseDto
    {
        public Guid CourseInstanceId { get; set; }
        //public List<CreateUpdateCourseAssignedStudentDto> Students { get; set; }
        public IEnumerable<long> Students { get; set; }
        public IEnumerable<string> StudentNames { get; set; }
    }
}
