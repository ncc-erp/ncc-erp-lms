using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserExtraRoles.Dto
{
    public class CourseAdminsToCourseDto
    {
        public Guid CourseId { get; set; }
        public List<long> CourseAdminIds { get; set; }
        public List<string> CourseAdminNames { get; set; }
    }
}
