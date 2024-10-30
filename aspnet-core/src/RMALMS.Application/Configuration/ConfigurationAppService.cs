using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using RMALMS.Configuration.Dto;
using RMALMS.Helper;

namespace RMALMS.Configuration
{
    [AbpAuthorize(Authorization.PermissionNames.Pages_Configurations)]
    public class ConfigurationAppService : RMALMSAppServiceBase, IConfigurationAppService
    {
        readonly IUploadHelper _uploadHelper;

        public ConfigurationAppService(IUploadHelper uploadHelper)
        {
            _uploadHelper = uploadHelper;
        }

        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        public async Task ChangeDirectionLocation(ChangeStorageLocationInput input)
        {

            if (!AbpSession.TenantId.HasValue)
            {
                return;
            }
            //var oldSetting = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.StorageLocation, AbpSession.TenantId.Value);
            //if (oldSetting != input.Location)
            //{
            var tenantId = AbpSession.TenantId.Value;
            await SettingManager.ChangeSettingForTenantAsync(tenantId, AppSettingNames.CourceFolder, input.Location);
            _uploadHelper.GetMediaFolderPath(input.Location);

            await SettingManager.ChangeSettingForTenantAsync(tenantId, AppSettingNames.SCORMCourceResourceFolder, input.ScormLocation);
            _uploadHelper.GetMediaFolderPath(input.ScormLocation, isSCORM:true);
            //}          

        }

        public async Task<ConfigDto> GetConfig()
        {

            var config = new ConfigDto();
            config.Location = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.CourceFolder, AbpSession.TenantId.Value);
            config.ScormLocation = await SettingManager.GetSettingValueForTenantAsync(AppSettingNames.SCORMCourceResourceFolder, AbpSession.TenantId.Value);
            return config;

        }
    }
}
