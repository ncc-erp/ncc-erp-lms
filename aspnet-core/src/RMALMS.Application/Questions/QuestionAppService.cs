using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Questions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Questions
{
    [AbpAuthorize]
    public class QuestionAppService : AsyncCrudAppService<Question, QuestionDto, Guid, PagedResultRequestDto, CreateQuestionDto, QuestionDto>, IQuestionAppService
    {
        private readonly IWorkScope _ws;
        public QuestionAppService(IRepository<Question, Guid> repository, IWorkScope workScope) : base(repository)
        {
            _ws = workScope;
        }

        public async Task<ListResultDto<QuestionDto>> GetQuestionsByModuleId(Guid moduleId)
        {
            throw new NotImplementedException();
            //var query = Repository.GetAll().Where(p => p.ModuleId == moduleId).ProjectTo<QuestionDto>();
            //var items = await query.ToListAsync();
            //return new ListResultDto<QuestionDto>(items);
        }

        [HttpPost]
        public async Task<GridResult<QuestionDto>> GetQuestionsByCourseIdPagging(GridCourseParam input)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == input.courseId).ProjectTo<QuestionDto>();
            return await query.GetGridResult(query, input.input);
        }

        public async override Task<QuestionDto> Update(QuestionDto input)
        {
            //check valid
            // only type matching can be duplicate rAnswer
            // other wise not
            if (input.Type != QuestionType.Matching)
            {
                var query = input.Answers.GroupBy(x => x.RAnswer).Where(g => g.Count() > 1).Select(y => y.Key).ToList<string>();
                if (query != null && !query.IsEmpty())
                {
                    throw new UserFriendlyException("Duplicated following answers: " + string.Join(", ", query.ToArray()));
                }
            }

            //scq has only 1 correct answer
            if (input.Type == QuestionType.SCQ)
            {
                
                //if (input.Answers.Count(s => s.IsCorrect.Value) != 1 )
                if(input.Answers.Where(s=>s.IsCorrect.HasValue && s.IsCorrect.Value == true).Count() != 1) 
                    throw new UserFriendlyException("Single choice question has to have 1 correct answer");
            }
            if (input.Type == QuestionType.MCQ)
            {
                // if (input.Answers.Count(s => s.IsCorrect.Value) < 1)
                if (input.Answers.Where(s => s.IsCorrect.HasValue && s.IsCorrect.Value == true).Count() < 1)
                    throw new UserFriendlyException("MCQ or SCQ has to have atleast one correct answer");
            }            
            //if valid -> save to db
            //save Mark to QuestionQuiz
            var qq = await _ws.GetRepo<QuestionQuiz>().GetAsync(input.QuestionQuizId);
            qq.Mark = input.Mark;
            await _ws.UpdateAsync(qq);

            //Save info to Question
            var item = await Repository.GetAsync(input.Id);
            MapToEntity(input, item);
            await Repository.UpdateAsync(item);
            var alreadyList = await _ws.GetAll<Answer, Guid>().Where(a => a.QuestionId == input.Id).ProjectTo<AnswerDto>().ToListAsync();

            var currentList = await (from a in _ws.GetAll<Answer>()
                                     where a.QuestionId == input.Id
                                     select a.Id).ToListAsync();

            //insert
            var insertList = input.Answers.Where(i => !currentList.Contains(i.Id));
            foreach (var ansDto in insertList)
            {
                var answer = ObjectMapper.Map<Answer>(ansDto);
                answer.QuestionId = item.Id;
                if (answer.LAnswer != null)
                {
                    answer.LAnswer = answer.LAnswer.Trim();
                }
                
                await _ws.InsertAsync(answer);
            }
            //update
            var updateList = input.Answers.Where(i => currentList.Contains(i.Id));
            var updateListIds = updateList.Select(u => u.Id);
            foreach (var ansDto in updateList)
            {
                var answer = ObjectMapper.Map<Answer>(ansDto);
                if (answer.LAnswer != null)
                {
                    answer.LAnswer = answer.LAnswer.Trim();
                }                
                await _ws.UpdateAsync(answer);
            }
            //delete
            var deleteList = currentList.Where(a => !updateListIds.Contains(a));
            foreach (var ansId in deleteList)
            {
                await _ws.GetRepo<Answer>().DeleteAsync(ansId);
            }
            return MapToEntityDto(item);
        }


        public async override Task<QuestionDto> Create(CreateQuestionDto input)
        {
            //check valid
            // only type matching can be duplicate rAnswer
            // other wise not

            if (input.Type != QuestionType.Matching)
            {
                var query = input.Answers.GroupBy(x => x.RAnswer).Where(g => g.Count() > 1).Select(y => y.Key).ToList<string>();
                if (query != null && !query.IsEmpty())
                {
                    throw new UserFriendlyException("Duplicated following answers: " + string.Join(", ", query.ToArray()));
                }
            }
            //scq has only 1 correct answer
            if (input.Type == QuestionType.SCQ)
            {
                if (input.Answers.Count(s => s.IsCorrect) != 1)
                    throw new UserFriendlyException("Single choice question has to have 1 correct answer");
            }
            else if (input.Type == QuestionType.MCQ)
            {
                if (input.Answers.Count(s => s.IsCorrect) < 1)
                    throw new UserFriendlyException("MCQ or SCQ has to have atleast one correct answer");
            }

            //if valid -> save to db
            var item = ObjectMapper.Map<Question>(input);            
            item.Id = await _ws.InsertAndGetIdAsync(item);

            await CurrentUnitOfWork.SaveChangesAsync();
            foreach (var answerDto in input.Answers)
            {
                var answer = ObjectMapper.Map<Answer>(answerDto);
                answer.QuestionId = item.Id;
                await _ws.InsertAsync(answer);
            }
            var CountQuestionQuiz = await _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == input.QuizId).CountAsync();
            var questionquiz = new QuestionQuiz
            {
                QuizId = input.QuizId,
                QuestionId = item.Id,
                Mark = input.Mark,
                Index=CountQuestionQuiz
            };

            var mapping = ObjectMapper.Map<QuestionQuiz>(questionquiz);
            await _ws.InsertAsync(mapping);

            return MapToEntityDto(item);
        }

        public async override Task<QuestionDto> Get(EntityDto<Guid> input)
        {
            var item = await Repository.GetAsync(input.Id);
            var itemDto = MapToEntityDto(item);
            var answers = _ws.GetAll<Answer, Guid>().Where(a => a.QuestionId == input.Id).ProjectTo<AnswerDto>();
            itemDto.Answers = await answers.ToListAsync();
            return itemDto;
        }
        [HttpPost]
        public async Task<GridResult<QuestionDto>> GetQuestionsByQuizIdPagging(GridCourseParam input)
        {
            var query =
                from q in _ws.GetAll<Question>()
                join a in _ws.GetAll<Answer>() on q.Id equals a.QuestionId into answers
                join qmq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == input.quizId) on q.Id equals qmq.QuestionId
                select new QuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Mark = qmq.Mark,
                    NWord = q.NWord,
                    Type = q.Type,
                    CourseId = q.CourseId,
                    Group = q.Group,
                    QuestionQuizId=qmq.Id,
                    Index=qmq.Index,
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
            query = query.OrderBy(s => s.Index).AsQueryable();
            return await query.GetGridResult(query, input.input);
        }
        public async Task SaveIndexQuestion(SaveIndexQuestionsDto input)
        {
            var OldIndex = await _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == input.QuizId).ToListAsync();
               foreach(var i in input.ListChange)
            {
                foreach(var j in OldIndex)
                {
                    if (i.Id == j.Id)
                    {
                        j.Index = i.Index;
                        await _ws.UpdateAsync(j);
                    }
                }
            }
              
        }
        public async Task<List<QuestionDto>> GetQuestionsByQuizIdNotPagging(Guid quizId)
        {
            var query =
                from q in _ws.GetAll<Question>()
                join a in _ws.GetAll<Answer>() on q.Id equals a.QuestionId into answers
                join qmq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == quizId) on q.Id equals qmq.QuestionId
                select new QuestionDto
                {
                    QuestionQuizId = qmq.Id,
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Mark = qmq.Mark,
                    NWord = q.NWord,
                    Type = q.Type,
                    CourseId = q.CourseId,
                    Group = q.Group,                    
                    Index=qmq.Index,
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
            query = query.OrderBy(s => s.Index).AsQueryable();
            return await query.ToListAsync();
        }


        public async Task<List<QuestionDto>> GetQuestionsByTestAttemptIdNotPagging(Guid testAttemptId)
        {
            var quizId = await _ws.GetRepo<TestAttempt>().GetAllIncluding(s => s.QuizSetting).Where(s => s.Id == testAttemptId).Select(s => s.QuizSetting.QuizId).FirstOrDefaultAsync();
            var qselectedQuestionIds = _ws.GetAll<StudentAnswer>()
                .Where(s => s.TestAttempId == testAttemptId)
                .Select(s => s.QuestionId)
                .Distinct();
            var qquestions =
                from qid in qselectedQuestionIds
                join a in _ws.GetAll<Answer>() on qid equals a.QuestionId into answers
                join qq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == quizId) on qid equals qq.QuestionId
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
            
            return await qquestions.ToListAsync();
        }


        public async Task<List<QuestionDto>> GetQuestionsByQuizSettingIdNotPagging(Guid quizSettingId)
        {
            var query =
                from q in _ws.GetAll<Question>()
                join a in _ws.GetAll<Answer>() on q.Id equals a.QuestionId into answers
                join qmq in _ws.GetAll<QuestionQuiz>() on q.Id equals qmq.QuestionId
                join qs in _ws.GetAll<QuizSetting>().Where(s => s.Id == quizSettingId) on qmq.QuizId equals qs.QuizId
                select new QuestionDto
                {
                    QuestionQuizId = qmq.Id,
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Mark = qmq.Mark,
                    NWord = q.NWord,
                    Type = q.Type,
                    CourseId = q.CourseId,
                    Group = q.Group,
                    Index=qmq.Index,
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
            query = query.OrderBy(s => s.Index).AsQueryable();
            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<GridResult<QuestionDto>> GetQuestionsPool(GridCourseParam input)
        {
            var query =
                   from q in _ws.GetAll<Question>().Where(s => s.CourseId == input.courseId)
                   join a in _ws.GetAll<Answer>() on q.Id equals a.QuestionId into answers
                   select new QuestionDto
                   {
                       Id = q.Id,
                       Title = q.Title,
                       Description = q.Description,
                       Mark = 1,
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
            return await query.GetGridResult(query, input.input);
        }

        [HttpGet]
        public async Task<QuestionDto> LinkQuestion(Guid quizId, Guid questionId)
        {
            var item = await Repository.GetAsync(questionId);
            if (item != null)
            {
                var questionquiz = new QuestionQuiz
                {
                    QuizId = quizId,
                    QuestionId = questionId,
                    Mark = 1,
                };
                var mapping = ObjectMapper.Map<QuestionQuiz>(questionquiz);
                await _ws.InsertAsync(mapping);
            }
            return MapToEntityDto(item);
        }
        public async Task RemoveLink(Guid quizId, Guid questionId)
        {
            var item = await Repository.GetAsync(questionId);
            if (item != null)
            {
                var questionquiz = _ws.GetAll<QuestionQuiz>().Where(qq => qq.QuizId == quizId && qq.QuestionId == questionId).FirstOrDefault();
                await _ws.GetRepo<QuestionQuiz>().DeleteAsync(questionquiz.Id);
            }
            return ;
        }
    }
}
