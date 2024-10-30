using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.CourseCategories.Dto
{
    [AutoMapTo(typeof(CourseTag))]
    public class CourseTagDto: EntityDto<Guid>
    {
        public Guid CourseId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
