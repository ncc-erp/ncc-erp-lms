using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace RMALMS.Authorization
{
    public class RMALMSAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Permission, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, RMALMSConsts.LocalizationSourceName);
        }
    }
}
