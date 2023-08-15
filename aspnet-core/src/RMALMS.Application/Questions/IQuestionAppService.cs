using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Pages.Dto;
using RMALMS.Paging;
using RMALMS.Questions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Questions
{
    public interface IQuestionAppService: IAsyncCrudAppService<QuestionDto, Guid, PagedResultRequestDto, CreateQuestionDto, QuestionDto>
    {
        Task<ListResultDto<QuestionDto>> GetQuestionsByModuleId(Guid moduleId);
        Task<GridResult<QuestionDto>> GetQuestionsByCourseIdPagging(GridCourseParam input);
        Task<GridResult<QuestionDto>> GetQuestionsPool(GridCourseParam input);
    }
}
