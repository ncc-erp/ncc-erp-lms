using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.UserGroups.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Roles.Dto;
using RMALMS.UserStatus.Dto;
using System.Threading.Tasks;
using RMALMS.Paging;

namespace RMALMS.UserGroups
{    
    public interface IUserGroupAppService: IAsyncCrudAppService<UserGroupDto, Guid, PagedResultRequestDto, CreateUserGroupDto, UserGroupDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task AddUsersToGroupAsync(UsersToGroupDto input);
        Task AddGroupsToUserAsync(GroupsToUserDto input);
        Task DeleteMulti(DeleteMultiUserGroupDto input);

        Task<GridResult<UserGroupDto>> GetAllAsync(GridParam input);
        Task <ListResultDto<UsersByGroupDto>> getUsersByGroupId(Guid groupId);

        Task <ListResultDto<GroupsByUsrerDto>> getGroupsByUserId(long userId);
    }
}
