using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace RMALMS.Groups.Dto
{

    [AutoMapTo(typeof(Group))]
    public class CreateGroupDto 
    {

        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Description { get; set; }

        public Guid? ParentId { get; set; }

    }
}
