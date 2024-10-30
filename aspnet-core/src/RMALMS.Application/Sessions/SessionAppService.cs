using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using RMALMS.Authorization.Roles;
using RMALMS.Sessions.Dto;

namespace RMALMS.Sessions
{
    public class SessionAppService : RMALMSAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                var user = await GetCurrentUserAsync();
                output.User = ObjectMapper.Map<UserLoginInfoDto>(user);
                if (user.TimeZoneId.HasValue)
                {
                    output.User.BaseUtcOffset = await UserManager.GetCurrentUtcOffsetAsync(user.TimeZoneId.Value);
                }

                var roleNames = await UserManager.GetUserRoleNames(user.Id);
                if (roleNames.Contains(StaticRoleNames.Tenants.Admin))
                {
                    output.User.RoleName = StaticRoleNames.Tenants.Admin;
                }
                else if (roleNames.Contains(StaticRoleNames.Tenants.CourseAdmin))
                {
                    output.User.RoleName = StaticRoleNames.Tenants.CourseAdmin;
                }
                else if (roleNames.Contains(StaticRoleNames.Tenants.Student))
                {
                    output.User.RoleName = StaticRoleNames.Tenants.Student;
                }

            }

            return output;
        }
    }
}
