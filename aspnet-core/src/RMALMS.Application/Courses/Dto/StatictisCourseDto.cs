using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class StatictisCourseDto
    {
        public int TotalCurrent { get; set; }
        public int TotalStartSoon { get; set; }
        public int TotalUpComming { get; set; }
        public int TotalArchived { get; set; }
        public int TotalExpired { get; set; }
        public List<StatictisCourseLevel> StatictisCourseLevels { get; set; }
        public List<StatictisCourseCategory> StatictisCourseCategory { get; set; }
    }

    public class StatictisCourseLevel
    {
        public string LevelName { get; set; }
        public int Total { get; set; }
    }

    public class StatictisCourseCategory
    {
        public string CategoryName { get; set; }
        public int Total { get; set; }
    }



}
