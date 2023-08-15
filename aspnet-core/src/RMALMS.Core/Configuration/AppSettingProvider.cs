using System;
using System.Collections.Generic;
using Abp.Configuration;

namespace RMALMS.Configuration
{   
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),

                // Tenant Setting
                new SettingDefinition(AppSettingNames.CourceFolder, "Cources", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.SCORMCourceResourceFolder, "SCORM", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                //new SettingDefinition(AppSettingNames.DashboardViewByStudent, "true", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                //new SettingDefinition(AppSettingNames.CalendarViewByStudent, "true", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.CourseEnrollRequireApproval, "true", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.ProficiencyLevelRequiredForEnroll, "true", scopes: SettingScopes.Tenant, isVisibleToClients: true),

                new SettingDefinition(AppSettingNames.StudentDefaultView,"0", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.DashboardDefaultView,"0", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor,"true", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.StudentProficiencyLevelRequired, "true", scopes: SettingScopes.Tenant, isVisibleToClients: true),
                // User setting
                //new SettingDefinition(AppSettingNames.UserPersonalInfoViewByPublic, "true", scopes: SettingScopes.User, isVisibleToClients: true),
                //new SettingDefinition(AppSettingNames.UserPersonalLinksViewByPublic, "true", scopes: SettingScopes.User, isVisibleToClients: true)
                new SettingDefinition(AppSettingNames.ClientAppId,"",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TimeScanFinishCourse,"10",scopes: SettingScopes.Application|SettingScopes.Tenant),
            };
        }
    }
}
