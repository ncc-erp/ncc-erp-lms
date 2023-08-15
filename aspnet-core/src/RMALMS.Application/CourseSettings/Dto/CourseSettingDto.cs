using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.CourseSettings.Dto
{
    [AutoMapTo(typeof(CourseInstance))]
    public class CourseInstanceDto: EntityDto<Guid>
    {
        public bool AllowSkip { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public float PassingMark { get; set; }        
        public Guid? CourseId { get; set; }        
        public int TotalQuiz { get; set; }
        //public CourseDto Course { get; set; }
        public bool EnableCourseGradingScheme { get; set; }
        public Guid? GradeSchemeId { get; set; }

    }
}
