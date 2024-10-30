using Abp.AspNetCore;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Abp.Timing;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RMALMS.Configuration;
using RMALMS.Constants;
using RMALMS.Identity;
using RMALMS.QAQuestionSignalR.SignalR;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Linq;
using System.Reflection;

namespace RMALMS.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            // Set Clock.Provider as UTC:
            Clock.Provider = ClockProviders.Utc;
            _appConfiguration = env.GetAppConfiguration();

        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConstantDefine.NUM_MINUTE_SCAN = _appConfiguration.GetValue<int>("TimeScanFinishCourse");

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName))
            );

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "RMALMS API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                
            });

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            //Get Config From AppSettings
            GetConfigAppSetting();

            // Configure Abp and Dependency Injection
            return services.AddAbp<RMALMSWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Remove("X-Frame-Options");
            //    await next();
            //});
            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAbpRequestLocalization();


            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
                routes.MapHub<QnAHub>("/signalr-qna");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


           

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(_appConfiguration["App:ServerRootAddress"].EnsureEndsWith('/') + "swagger/v1/swagger.json", "RMALMS API V1");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("RMALMS.Web.Host.wwwroot.swagger.ui.index.html");
            }); // URL: /swagger

            // set root folder
            WebContentDirectoryFinder.RootFolder = env.ContentRootPath;

            //app.Use((context, next) =>
            //{

            //    context.Response.Headers["X-Frame-Options"] = "ALLOW-FROM http://localhost:4200";
            //    return next.Invoke();
            //});
        }

        private void GetConfigAppSetting()
        {
            RMALMSConsts.ServerRootAddress = _appConfiguration.GetValue<string>("App:ServerRootAddress");
            RMALMSConsts.IsEnableMultiTenant = _appConfiguration.GetValue<bool>("EnableMultiTenant");
            RMALMSConsts.SercurityCode = _appConfiguration.GetValue<string>("SercurityCode");
        }
    }
}
