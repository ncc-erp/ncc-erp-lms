using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Scorms.Dto
{
    [AutoMapTo(typeof(StudentScorm))]
    public class StudentScormDto: EntityDto<Guid>
    {
        public Guid CourseAssignedStudentId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
