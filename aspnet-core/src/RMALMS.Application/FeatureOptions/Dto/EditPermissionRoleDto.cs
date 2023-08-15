using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.FeatureOptions.Dto
{
    public class EditPermissionRoleDto : EntityDto<long>
    {
        public bool IsGranted { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}
