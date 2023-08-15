using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using RMALMS.Authorization.Roles;
using RMALMS.Entities;
using System.Threading.Tasks;
using RMALMS.IoC;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RMALMS.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly IWorkScope _ws;
        public UserManager(
            IWorkScope workScope,
            RoleManager roleManager,
            UserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager)
            : base(
                roleManager,
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                organizationUnitRepository,
                userOrganizationUnitRepository,
                organizationUnitSettings,
                settingManager)
        {
            this._ws = workScope;
        }

        public Task<IdentityResult> CreateAsync(Group group, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetCurrentUtcOffsetAsync(Guid timeZoneId)
        {
            var timeZone = await _ws.GetRepo<UserTimeZone, Guid>().GetAll().IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Id == timeZoneId);
            if (timeZone == null)
            {
                return null;
            }
            else
            {
                TimeSpan ts = TimeSpan.Parse(timeZone.BaseUtcOffset);
                if (ts > TimeSpan.Zero)
                    return "+" + ts.ToString("hhmm");
                else return "-" + ts.ToString("hhmm");
            }
        }

        public async Task<string> GetUserRoleNames(long userId)
        {
            var quser =
                    from ur in _ws.GetAll<UserRole, long>().Where(u => u.UserId == userId)
                    join role in _ws.GetAll<Role, int>()
                    on ur.RoleId equals role.Id into roles
                    from r in roles
                    select r.Name;
            var roleNames = await quser.ToListAsync();
            return string.Join("|", roleNames);
        }

    }
}
