using Abp.Auditing;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Configuration;
using RMALMS.Localization;
using RMALMS.MultiTenancy;
using RMALMS.Timing;

namespace RMALMS
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class RMALMSCoreModule : AbpModule
    {

        public override void PreInitialize()
        {
            //Configuration.ReplaceService<IAuditingStore, MyAuditingStore>(DependencyLifeStyle.Transient);
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            RMALMSLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = RMALMSConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RMALMSCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
