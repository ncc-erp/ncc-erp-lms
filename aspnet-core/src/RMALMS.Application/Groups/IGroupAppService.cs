using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Entities;
using RMALMS.Groups.Dto;
using RMALMS.Roles.Dto;
using RMALMS.Users.Dto;

namespace RMALMS.Groups
{
    public interface IGroupAppService : IAsyncCrudAppService<GroupDto, Guid, PagedResultRequestDto, CreateGroupDto, GroupDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task<ListResultDto<GroupDto>> GetAllGroups();//Not paging for dropdownlist
    }
}
