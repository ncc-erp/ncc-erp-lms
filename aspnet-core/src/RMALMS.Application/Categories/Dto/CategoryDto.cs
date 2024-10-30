using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Categories.Dto
{
    [AutoMapTo(typeof(Category))]
    public class CategoryDto: EntityDto<Guid>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        [ApplySearchAttribute]
        public string Name { get; set; }
        [ApplySearchAttribute]
        public string Description { get; set; }
    }
}
