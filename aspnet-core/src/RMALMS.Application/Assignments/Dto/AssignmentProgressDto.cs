using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RMALMS.Assignments.Dto
{
    public class AssignmentProgressDto : EntityDto<Guid>
    {
        [ApplySearchAttribute]
        public string Title { get; set; }
        public float? MaxPoint { get; set; }
        public float? Point { get; set; }
        public Guid AssignmentSettingId { get; set; }
    }
}
