using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Authorization;
using System.Collections.Generic;

namespace RMALMS.Roles.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class PermissionDto : EntityDto<long>
    {
        public string Name { get; set; }

        public bool IsGranted { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public IEnumerable<RoleDto> Role { get; set; }
    }
}
