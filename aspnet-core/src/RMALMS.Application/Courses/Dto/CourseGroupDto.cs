using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class CourseGroupDto : EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid CourseInstanceId { get; set; }
    }

    public class CourseGroupWithMemberDto : EntityDto<Guid>
    {
        public string GroupName { get; set; }
        public IEnumerable<StudentCourseGroupListDto> Students { get; set; }
        public bool Selected { get; set; }
        public bool IsEveryOne { get; set; }
    }
}
