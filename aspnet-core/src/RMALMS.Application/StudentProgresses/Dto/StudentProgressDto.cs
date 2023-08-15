using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.StudentProgresses.Dto
{
    [AutoMapTo(typeof(StudentProgress))]
    public class StudentProgressDto: EntityDto<Guid>
    {        
        public Guid? PageId { get; set; }
        public StudentProgressStatus Progress { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
}
