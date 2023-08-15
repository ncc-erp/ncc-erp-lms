using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using RMALMS.Authorization;
using RMALMS.AutoMapper;
using RMALMS.Reports;
using Abp.Configuration.Startup;

namespace RMALMS
{
    [DependsOn(
        typeof(RMALMSCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class RMALMSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IAuditingStore, CustomizeAuditLog>(DependencyLifeStyle.Transient);
            Configuration.Authorization.Providers.Add<RMALMSAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(RMALMSApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
