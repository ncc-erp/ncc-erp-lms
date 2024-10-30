using Abp.Authorization;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;

namespace RMALMS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
