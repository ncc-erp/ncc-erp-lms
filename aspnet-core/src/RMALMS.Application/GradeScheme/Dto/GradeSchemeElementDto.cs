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
    [AutoMapTo(typeof(GradeSchemeElement))]
    public class GradeSchemeElementDto : EntityDto<Guid>
    {
        public Guid GradeSchemeId { get; set; }
        public float LowRange { get; set; }
        public float HighRange { get; set; }
        public string Name { get; set; }
        public CompareOperation LowCompareOperation { get; set; }
        public CompareOperation HighCompareOpertion { get; set; }

    }
}
