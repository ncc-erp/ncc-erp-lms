using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.TestAttempts.Dto
{
    [AutoMapTo(typeof(ScormTestAttempt))]
    public class ScormTestAttemptDto: EntityDto<Guid>
    {
        public string Name { get; set; }
        public float Score { get; set; }
        public float MaxScore { get; set; }
        public Guid CourseAssignedStudentId { get; set; }

        public bool IsFinal { get; set; }
    }    
}
