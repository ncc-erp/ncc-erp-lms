using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.GradeSchemes.Dto;
using RMALMS.Paging;
using RMALMS.Roles.Dto;

namespace RMALMS.GradeSchemes
{
    public interface IGradeSchemeAppService : IAsyncCrudAppService<GradeSchemeDto, Guid, PagedResultRequestDto, CreateGradeSchemeDto, GradeSchemeDto>
    {
        ListResultDto<PermissionDto> GetAllPermissions();
        Task<List<GradeSchemeDto>> GetGradesByCourseId(Guid courseId);
        Task<GridResult<GradeSchemeDto>> getGradesByCourseIdPagging(GridGradeParam input);

    }
}
