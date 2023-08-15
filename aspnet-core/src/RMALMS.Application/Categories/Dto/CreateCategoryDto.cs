using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Entities;

namespace RMALMS.Categories.Dto
{
    [AutoMapTo(typeof(Category))]
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string Name { get; set; }
        
        public string Description { get; set; }        

    }
}
