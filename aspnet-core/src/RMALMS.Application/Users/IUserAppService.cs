using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using RMALMS.Users.Dto;

namespace RMALMS.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
        Task ResetPasswordAsync(ResetPasswordDto input);
        Task<GridResult<UserDto>> GetUsersByTenantIdAsync(UsersByTenantIdDto input);
        Task<GridResult<UserBrifDto>> GetAllBrif(GridParam input);        
        Task<UserDto> GetUserById();
    }
}
