using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Filters;
using RMALMS.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Ncc
{
    public class NccAllowAnonymousAttribute : ActionFilterAttribute
    {
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _abpSession;
        public NccAllowAnonymousAttribute()
        {
            _tenantManager = IocManager.Instance.Resolve<TenantManager>();
            _abpSession = NullAbpSession.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var tenantName = RMALMSConsts.DefaultTenantName;
            if (RMALMSConsts.IsEnableMultiTenant)
            {
                var header = context.HttpContext.Request.Headers;
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
