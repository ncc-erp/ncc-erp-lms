using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Categories.Dto;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Categories
{
    public interface ICategoryAppService: IAsyncCrudAppService<CategoryDto, Guid, PagedResultRequestDto, CreateCategoryDto, CategoryDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task<ListResultDto<CategoryDto>> GetAllNotPagging();//Not paging for dropdownlist

        Task<GridResult<CategoryDto>> GetAllPagging(GridParam input);
    }
}
