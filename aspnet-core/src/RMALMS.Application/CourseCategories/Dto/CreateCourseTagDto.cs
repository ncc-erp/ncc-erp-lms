using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.CourseCategories.Dto
{
    [AutoMapTo(typeof(CourseTag))]
    public class CreateCourseTagDto
    {
        public Guid CourseId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
