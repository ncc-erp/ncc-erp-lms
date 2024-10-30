using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Authorization.Roles;
using RMALMS.Entities;

namespace RMALMS.Groups.Dto
{
    [AutoMapTo(typeof(Group))]
    public class GroupDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        [ApplySearchAttribute]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        [ApplySearchAttribute]
        public string Description { get; set; }
        
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
