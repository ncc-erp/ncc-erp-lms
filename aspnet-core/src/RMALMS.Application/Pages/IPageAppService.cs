using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Pages.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Pages
{
    public interface IPageAppService: IAsyncCrudAppService<PageDto, Guid, PagedResultRequestDto, CreatePageDto, PageDto>
    {
        Task<ListResultDto<PagesByModuleDto>> getPagesByModuleId(Guid moduleId);
    }
}
