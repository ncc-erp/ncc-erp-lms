using System.Threading.Tasks;
using RMALMS.Configuration.Dto;

namespace RMALMS.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
        Task ChangeDirectionLocation(ChangeStorageLocationInput input);
    }
}
