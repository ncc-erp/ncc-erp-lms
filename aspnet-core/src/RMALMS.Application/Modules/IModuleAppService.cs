using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Modules.Dto;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Modules
{
    public interface IModuleAppService: IAsyncCrudAppService<ModuleDto, Guid, PagedResultRequestDto, CreateModuleDto, ModuleDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task<ListResultDto<ModuleDto>> GetAllByCourseId(Guid courseId);

        Task<GridResult<ModuleDto>> GetAllPagging(GridParam input);

    }
}
