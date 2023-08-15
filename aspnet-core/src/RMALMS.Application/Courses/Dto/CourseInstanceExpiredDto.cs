using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class CourseInstanceExpiredDto : EntityDto<Guid>
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid CourseId { get; set; }
        public String CreaterName { get; set; }

    }
}
