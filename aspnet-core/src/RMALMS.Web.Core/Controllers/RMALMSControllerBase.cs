using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace RMALMS.Controllers
{
    public abstract class RMALMSControllerBase: AbpController
    {
        protected RMALMSControllerBase()
        {
            LocalizationSourceName = RMALMSConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
