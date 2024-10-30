using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Mvc.Filters;
using RMALMS.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Ncc
{
    public class NccAuthAttribute : ActionFilterAttribute
    {
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _abpSession;
        public NccAuthAttribute()
        {
            _tenantManager = IocManager.Instance.Resolve<TenantManager>();
            _abpSession = NullAbpSession.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var securityCode = RMALMSConsts.SercurityCode;
            var header = context.HttpContext.Request.Headers;
            var securityCodeHeader = header["X-Secret-Key"].ToString();
            if (securityCode != securityCodeHeader)
                throw new UserFriendlyException($"SecretCode does not match! LMSCode: {securityCode.Substring(securityCode.Length - 3)} != {securityCodeHeader}");

            var tenantName = RMALMSConsts.DefaultTenantName;
            if (RMALMSConsts.IsEnableMultiTenant)
            {
                tenantName = header["Abp-TenantName"].ToString();
                if (string.IsNullOrEmpty(tenantName))
                    return;
            }
            var tenant = _tenantManager.FindByTenancyName(tenantName);
            if (tenant == null)
                throw new Exception($"Not Found Tenant.");

            _abpSession.Use(tenant.Id, null);
        }
    }
}
