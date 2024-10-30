using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;

namespace RMALMS.Assignments.Dto
{
    [AutoMapTo(typeof(AssignmentSetting))]
    public class AssignmentSettingsDto : EntityDto<Guid>
    {
        public int? NumberOfDueDays { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public Guid CourseInstanceId { get; set; }
        public float? Point { get; set; }
    }
}
