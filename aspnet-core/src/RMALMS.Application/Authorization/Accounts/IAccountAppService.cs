using System.Threading.Tasks;
using Abp.Application.Services;
using RMALMS.Authorization.Accounts.Dto;

namespace RMALMS.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
        Task<UserInfoDto> GetUserProfile();
        Task<UserInfoDto> UpdateUserInfo(UserInfoDto userInfo);
    }
}
