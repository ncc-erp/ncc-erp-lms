using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    [AutoMapTo(typeof(CourseLevel))]
    public class CourseLevelDto : EntityDto<Guid>
    {
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public int? TenantId { get; set; }
        public int Level { get; set; }
        public bool IsStatic { get; set; }
        public CompareOperation? LowCompareOperation { get; set; }
        public int? RequiredStudentLevel { get; set; }
    }
}
