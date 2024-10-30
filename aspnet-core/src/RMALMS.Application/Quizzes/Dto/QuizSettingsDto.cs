using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;

namespace RMALMS.Quizzes.Dto
{
    [AutoMapTo(typeof(QuizSetting))]
    public class QuizSettingsDto: EntityDto<Guid>
    {
        public float? Point { get; set; }
        public int? NoOfDueDays { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public Guid CourseInstanceId { get; set; }
        public int TotalNumberQuestion { get; set; }
        public bool ApplySameStartEndTimeAsCourse { get; set; }
    }
}
