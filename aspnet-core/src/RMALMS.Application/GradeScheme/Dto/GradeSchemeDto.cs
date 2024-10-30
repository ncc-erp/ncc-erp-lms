using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Authorization.Roles;
using RMALMS.Entities;

namespace RMALMS.GradeSchemes.Dto
{
    [AutoMapTo(typeof(GradeScheme))]
    public class GradeSchemeDto : EntityDto<Guid>
    {
        [ApplySearchAttribute]
        public string Title { get; set; }
        public Guid CourseId { get; set; }
        public GradeSchemeStatus Status { get; set; }
        public List<GradeSchemeElementDto> ElementList { get; set; }

    }
}
