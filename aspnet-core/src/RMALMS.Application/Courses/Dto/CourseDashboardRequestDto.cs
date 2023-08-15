using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class CourseDashboardRequestDto : GridParam
    {
        public Guid CourseId { get; set; }
    }
}
