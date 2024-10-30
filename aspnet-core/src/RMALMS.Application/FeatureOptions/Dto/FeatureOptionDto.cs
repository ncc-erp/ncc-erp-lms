using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using RMALMS.Authorization.Roles;
using RMALMS.Configuration;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.FeatureOptions.Dto
{
    public class FeatureOptionDto 
    {
        public int StudentDefaultViewName { get; set; }
        public int DashboardDefaultViewName { get; set; }
        public bool StudentCourseEnrollment { get; set; }
        public bool StudentProficiency { get; set; }
        public List<PermissionRoleDto> PermissionRoles { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}
