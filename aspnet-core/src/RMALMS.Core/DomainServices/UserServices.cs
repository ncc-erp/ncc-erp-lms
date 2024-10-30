using Abp.Authorization.Users;
using Abp.Domain.Services;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMALMS.DomainServices
{
    public class UserServices : BaseDomainService, IUserServices
    {
        public IQueryable<User> GetUserByRole(string roleName)
        {
            var quser =
                from u in WorkScope.GetAll<User, long>()
                join r in
                    from ur in WorkScope.GetAll<UserRole, long>()
                    join role in WorkScope.GetAll<Role, int>().Where(s => s.Name == roleName)
                    on ur.RoleId equals role.Id
                    select ur.UserId
                on u.Id equals r into roles
                where roles.Any()
                select u;

            return quser;
        }

        public bool UserHasRole(long userId, string roleName)
        {
            var quser =
                    from ur in WorkScope.GetAll<UserRole, long>().Where(u => u.UserId == userId)
                    join role in WorkScope.GetAll<Role, int>().Where(s => s.Name == roleName)
                    on ur.RoleId equals role.Id into roles
                    from r in roles
                    select r.Id;
            return quser.Any();

        }
    }
}
