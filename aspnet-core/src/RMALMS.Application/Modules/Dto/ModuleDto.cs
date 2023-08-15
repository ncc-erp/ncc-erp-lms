using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Modules.Dto
{
    [AutoMapTo(typeof(Module))]
    public class ModuleDto: EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int SequenceOrder { get; set; }
        public Guid CourseId { get; set; }
    }
}
