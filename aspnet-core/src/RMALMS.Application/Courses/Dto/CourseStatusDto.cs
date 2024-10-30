using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    [AutoMapTo(typeof(CourseLevel))]
    public class CourseStatusDto: EntityDto<Guid>
    {
        public int Level { get; set; }
        public string DisplayName { get; set; }
    }
}
