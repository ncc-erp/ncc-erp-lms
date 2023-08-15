using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Assignments.Dto;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Assignments
{
    public interface IAssignmentAppService : IAsyncCrudAppService<AssignmentDto, Guid, PagedResultRequestDto, CreateAssignmentDto, AssignmentDto>
    {
        Task<List<AssignmentDto>> GetAssignmentsByCourseId(Guid courseId);
        Task<GridResult<AssignmentDto>> GetAssignmentsByCourseIdPagging(GridAssignmentsParam input);
    }
}
