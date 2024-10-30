using RMALMS.Entities;
using RMALMS.TestAttempts.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using RMALMS.Questions.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using RMALMS.DomainServices;
using RMALMS.Uitls;
using Abp.UI;

namespace RMALMS.TestAttempts
{
    [AbpAuthorize]
    public class TestAttemptAppService : ApplicationBaseService
    {
        private readonly IQuizManager _quizManager;
        public TestAttemptAppService(
            IQuizManager quizManager
        ) : base()
        {
            this._quizManager = quizManager;
        }
        public async Task<TestAttemptDto> Create(TestAttemptDto input)
        {
            var quizSetting = await _ws.GetRepo<QuizSetting>().GetAsync(input.QuizSettingId);
            if (quizSetting == null)
                throw new UserFriendlyException(String.Format("QuizSetting id {0} is not exist", input.QuizSettingId));

            var item = ObjectMapper.Map<TestAttempt>(input);

            item.CourseAssignedStudentId = 
                await (from cas in _ws.GetAll<CourseAssignedStudent>()
                       .Where(s => s.Status == AssignedStatus.Accepted && s.StudentId == AbpSession.UserId.Value && s.CourseInstanceId == quizSetting.CourseInstanceId)
                                                  orderby cas.CreationTime descending
                                                  select cas.Id).FirstOrDefaultAsync();
            if (item.CourseAssignedStudentId == null)
                throw new UserFriendlyException(String.Format("User id {0} has not accepted to do this quiz", AbpSession.UserId.Value));

            item.LastModificationTime = DateTimeUtils.GetNow();
            input.Id = await _ws.InsertAndGetIdAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            Random random = new Random();
            //create studentAnswers - for quiz option: total question number
            var qquestionQuiz = _ws.GetRepo<QuestionQuiz>().GetAllIncluding(s => s.Question).Where(s => s.QuizId == quizSetting.QuizId)
                .Select(s => new { s.QuestionId, Order = random.Next(int.MaxValue), Point = s.Mark, s.Question.Title });
                
            if (quizSetting.TotalNumberQuestion > 0)
            {
                qquestionQuiz = qquestionQuiz.OrderBy(s => s.Order).Take(quizSetting.TotalNumberQuestion);
            }
            var randomQuestions = await qquestionQuiz.ToListAsync();
            foreach (var questionId in randomQuestions)
            {
                var sa = new StudentAnswer
                {
                    QuestionId = questionId.QuestionId,
                    Mark = 0,
                    TestAttempId = item.Id,
                    Status = StudentAnswerStatus.JustInitial
                };
                await _ws.InsertAsync(sa);
            }
            item.MaxScore = randomQuestions.Sum(s => s.Point);
            await _ws.UpdateAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            var qselectedQuestionIds = _ws.GetAll<StudentAnswer>().Where(s => s.TestAttempId == item.Id && s.Status == StudentAnswerStatus.JustInitial).Select(s => s.QuestionId);           
            var qquestions =
                from qid in qselectedQuestionIds
                join a in _ws.GetAll<Answer>() on qid equals a.QuestionId into answers
                join qq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == quizSetting.QuizId) on qid equals qq.QuestionId
                join q in _ws.GetAll<Question>() on qid equals q.Id                
                select new QuestionDto
                {
                    QuestionQuizId = qq.Id,
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Mark = qq.Mark,
                    NWord = q.NWord,
                    Type = q.Type,
                    CourseId = q.CourseId,
                    Group = q.Group,
                    Answers = answers.Select(
                        s => new AnswerDto
                        {
                            Id = s.Id,
                            IsCorrect = s.IsCorrect,
                            LAnswer = s.LAnswer,
                            QuestionId = s.QuestionId,
                            RAnswer = s.RAnswer,
                            SequenceOrder = s.SequenceOrder
                        }).ToList()
                };
            //
            var itemDto = ObjectMapper.Map<TestAttemptDto>(item);
            itemDto.Questions = await qquestions.ToListAsync();
            return itemDto;
        }

        public async Task<TestAttemptDto> Update(TestAttemptDto input)
        {
            var item = await _ws.GetRepo<TestAttempt>().GetAsync(input.Id);
            item.Status = input.Status;
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<TestAttemptDto>(item);
        }

        public async Task<TestAttemptDto> ProcessScore(TestAttemptDto input)
        {
            var studentId = AbpSession.UserId;
            var item = await _quizManager.ProcessScore(studentId.Value, input.Id, input.Type);
            var itemDto = ObjectMapper.Map<TestAttemptDto>(item);
            return itemDto;
        }

        public async Task<TestAttemptDto> GetTestAttempt(Guid quizSettingId, TestAttemptStatus status)
        {
            var items = _ws.GetAll<TestAttempt>().Where(ta => ta.CreatorUserId == AbpSession.UserId.Value && ta.QuizSettingId == quizSettingId);

            var item = await items.Where(ta => ta.Status == status).LastOrDefaultAsync();
            var itemDto = ObjectMapper.Map<TestAttemptDto>(item);

            if (item != null)
            {
                var timeLimit = await _ws.GetRepo<QuizSetting>().GetAllIncluding(qs => qs.Quiz).Where(qs => qs.Id == quizSettingId && qs.Quiz.TimeLimit.HasValue).Select(qs => qs.Quiz.TimeLimit).FirstOrDefaultAsync();
                if (timeLimit != null && timeLimit > 0)
                {
                    int timePass = (int)DateTime.Now.Subtract(item.CreationTime).TotalSeconds;
                    itemDto.TimeRemaining = timeLimit * 60 - timePass;
                }
            }


            return itemDto;
        }

        public async Task<List<TestAttemptDto>> GetStudentTestAttempts(long studentId, Guid quizSettingId)
        {
            return await _ws.GetAll<TestAttempt>().Where(ta => ta.CreatorUserId == studentId && ta.QuizSettingId == quizSettingId && ta.Status == TestAttemptStatus.Marking).ProjectTo<TestAttemptDto>().ToListAsync();
        }

        //public async Task<TestAttemptDto> UpdateStudentPoint()

    }
}
