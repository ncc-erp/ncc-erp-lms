using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.Pages.Dto;
using RMALMS.Paging;
using RMALMS.Quizzes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Quizzes
{
    public interface IQuizAppService: IAsyncCrudAppService<QuizDto, Guid, PagedResultRequestDto,CreateQuizDto, QuizDto>
    {
        Task<GridResult<QuizDto>> GetQuizzesByCourseIdPagging(GridQuizzesParam input);
        Task<List<QuizDto>> GetQuizzesByCourseId(Guid courseId);
        Task<QuizDto> GetQuizDataById(Guid courseId,Guid courseInstanceId);

    }
}
