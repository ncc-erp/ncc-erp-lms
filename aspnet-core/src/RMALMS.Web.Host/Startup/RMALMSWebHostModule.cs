using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using RMALMS.Configuration;
using Abp.Threading.BackgroundWorkers;
using RMALMS.BackgroundWorkers;

namespace RMALMS.Web.Host.Startup
{
    [DependsOn(
       typeof(RMALMSWebCoreModule))]
    public class RMALMSWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public RMALMSWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RMALMSWebHostModule).GetAssembly());
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<FinishCoursesBackgroundWorker>());
        }
    }
}
