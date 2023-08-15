using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.CourseCategories.Dto
{
    public class TagsByCourseDto: EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
