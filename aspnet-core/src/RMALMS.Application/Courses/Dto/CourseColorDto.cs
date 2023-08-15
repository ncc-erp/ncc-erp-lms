using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    [AutoMapTo(typeof(CourseColor))]
    public class CourseColorDto
    {
        public string ColorCode { get; set; }
        public Guid CourseId { get; set; }
    }
}
