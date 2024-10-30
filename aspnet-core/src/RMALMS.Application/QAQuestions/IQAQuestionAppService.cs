using Abp.Application.Services.Dto;
using RMALMS.Paging;
using RMALMS.QAQuestions.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMALMS.QAQuestions
{
    public interface IQAQuestionAppService
    {
        Task<QAQuestionOut> CreateQAQuestion(QAQuestionCreateInput input);
        Task<QAAnswersDto> CreateQAAnswer(QAAnswerInput input);
        Task<QAAnswersDto> UpdateQAAnswer(IdContentInput input);
        Task<GridResult<QAQuestionOut>> GetQAQuestionAnswer(GridQAAnswerParam input);
        Task<ListResultDto<QAAnswersDto>> GetQAAnswerByQAQuestionId(Guid QAQuestionId);
        Task<Boolean> AddUserFollowQA(IdContentInput input);
        Task<Boolean> DeleteUserFollowQA(IdContentInput input);
        Task<GridResult<DiscussionOut>> GetsDiscussionByCourseInstanceIdPagging(GridDiscussionParam input);
        Task<bool> DeleteQAQuestion(IdContentInput input);
        Task<bool> DeleteQAAnswer(IdContentInput input);
        Task<bool> DeleteFAQQuestion(IdContentInput input);
        Task<ListResultDto<FAQQuestionDto>> GetsFAQQuestionByCourseId(IdContentInput input);
        Task<ListResultDto<FAQQnADto>> GetsFAQQnAByCourseId(Guid CourseId);
        Task<FAQQuestionDto> CreateFAQQuestion(FAQQuestionInput input);
        Task<FAQQuestionDto> UpdateFAQQuestion(FAQQuestionInput input);
        Task<FAQAnswerDto> CreateFAQAnswer(FAQAnswerInput input);
        Task<Boolean> UpdateFAQQuestionSequenceOrder(IEnumerable<FAQSequenceOrderInput> inputs);
    }
}
