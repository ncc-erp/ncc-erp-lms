using RMALMS.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.FeatureOptions.Dto
{
    public class DefaultViewDto
    {
        public int StudentDefaultViewName { get; set; }
        public int DashboardDefaultViewName { get; set; }
        public bool StudentCourseEnrollment { get; set; }
        public bool StudentProficiency { get; set; }
    }
}
