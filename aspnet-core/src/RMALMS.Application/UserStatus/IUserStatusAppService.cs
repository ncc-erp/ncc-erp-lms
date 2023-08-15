using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Roles.Dto;
using RMALMS.UserStatus.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.UserStatus
{
    public interface IUserStatusAppService: IAsyncCrudAppService<UserStatusDto, Guid, PagedResultRequestDto, CreateUserStatusDto, UserStatusDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task<ListResultDto<UserStatusDto>> GetAllNotPagging();//Not pagging for dropdownlist
    }
}
