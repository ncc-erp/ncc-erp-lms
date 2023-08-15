using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Scorms.Dto
{
    [AutoMapTo(typeof(StudentProgressScorm))]
    public class StudentProgressScormDto: EntityDto<Guid>
    {
        public Guid CourseAssignedStudentId { get; set; }
        public string PageId { get; set; }
        public StudentProgressStatus  Progress { get; set; }
    }
}
