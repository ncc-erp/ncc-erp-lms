using RMALMS.Questions.Dto;
using RMALMS.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using RMALMS.Extension;
using RMALMS.Paging;
using Microsoft.AspNetCore.Mvc;
using Abp.UI;
using RMALMS.TestAttempts.Dto;
using RMALMS.DomainServices;

namespace RMALMS.Questions
{
    [AbpAuthorize]
    public class StudentAnswerAppService : ApplicationBaseService
    {
        private readonly IUserCertificationManager _userCertificationManager;
        public StudentAnswerAppService(IUserCertificationManager userCertificationManager) : base()
        {
            this._userCertificationManager = userCertificationManager;
        }

        public async Task<StudentAnswerDto> Create(StudentAnswerDto input)
        {
            var isExist = await _ws.GetAll<StudentAnswer>().AnyAsync(sa => sa.QuestionId == input.QuestionId && sa.CreatorUserId == AbpSession.UserId.Value && sa.TestAttempId == input.TestAttempId);
            if (isExist)
                throw new UserFriendlyException(String.Format("You did answer for question Id {0}", input.QuestionId));
            var item = ObjectMapper.Map<StudentAnswer>(input);
            item.Status = StudentAnswerStatus.StudentAnswer;
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<StudentAnswerDto>(item);
        }

        public async Task<StudentAnswerDto> Update(StudentAnswerDto input)
        {
            var item = _ws.GetRepo<StudentAnswer>().Get(input.Id);
            ObjectMapper.Map<StudentAnswerDto, StudentAnswer>(input, item);
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<StudentAnswerDto>(item);
        }

        public async Task<StudentAnswerQuestionDto> Save(StudentAnswerQuestionDto input)
        {
            var studentAnswerIdList = await _ws.GetAll<StudentAnswer>()
                .Where(sa => sa.QuestionId == input.QuestionId)
                .Where(sa => sa.CreatorUserId == AbpSession.UserId.Value)
                .Where(sa => sa.TestAttempId == input.TestAttempId)
                .Select(s => s.Id)
                .ToListAsync();

            foreach (var id in studentAnswerIdList)
            {
                await _ws.DeleteAsync<StudentAnswer>(id);
            }

            foreach (var item in input.StudentAsnwers)
            {
                var studentAswer = new StudentAnswer
                {
                    QuestionId = input.QuestionId,
                    TestAttempId = input.TestAttempId,
                    AnswerId = item.AnswerId,
                    AnswerText = item.AnswerText,
                    Status = StudentAnswerStatus.StudentAnswer
                };
                item.Id = await _ws.InsertAndGetIdAsync(studentAswer);
            }

            return input;
        }


        public async Task CreateStudentAnswers(List<StudentAnswerDto> input)
        {
            if (input != null && input.Count > 0)
            {
                var holdQuestionId = new List<Guid?>();
                var holdTestAttempId = new List<Guid?>();
                var holdAnswerId = new List<Guid?>();
                foreach (var item in input)
                {
                    var answerOfQuestion = await _ws.GetAll<StudentAnswer>()
                                                    .FirstOrDefaultAsync(sa => sa.QuestionId == item.QuestionId
                                                    && sa.Status == StudentAnswerStatus.StudentAnswer
                                                    && sa.TestAttempId == item.TestAttempId
                                                    && (sa.AnswerId.HasValue && sa.AnswerId == item.AnswerId));
                    //if answer not exist on db => add to insert list
                    if (answerOfQuestion == default)
                    {
                        //insertQuestionIds.Add(item.QuestionId);
                        var stn = ObjectMapper.Map<StudentAnswer>(item);
                        stn.Status = StudentAnswerStatus.StudentAnswer;
                        await _ws.InsertAsync(stn);
                    }
                    holdQuestionId.Add(item.QuestionId);
                    holdTestAttempId.Add(item.TestAttempId);
                    holdAnswerId.Add(item.AnswerId);
                }

                // delete answer not choice
                var deleteAnswerOfQuestion = await _ws.GetAll<StudentAnswer>()
                                                    .Where(sa => holdQuestionId.Contains(sa.QuestionId)
                                                    && sa.Status == StudentAnswerStatus.StudentAnswer
                                                    && holdTestAttempId.Contains(sa.TestAttempId)
                                                    && (sa.AnswerId.HasValue && !holdAnswerId.Contains(sa.AnswerId))).ToListAsync();
                foreach(var item in deleteAnswerOfQuestion)
                {
                    await _ws.DeleteAsync<StudentAnswer>(item.Id);
                }
            }

        }

        [HttpPost]
        public async Task<GridResult<StudentAnswerDto>> GetStudentAnswersPagging(GridStudentAnswerDto input)
        {
            var query = _ws.GetAll<StudentAnswer>()
                .Where(sa => sa.Status == StudentAnswerStatus.StudentAnswer && sa.CreatorUserId == AbpSession.UserId.Value && sa.TestAttempId == input.TestAttempId)
                .ProjectTo<StudentAnswerDto>();
            return await query.GetGridResult(query, input.input);
        }

        public async Task<List<StudentAnswerDto>> GetStudentAnswersNotPagging(Guid testAttemptId)
        {
            return await _ws.GetAll<StudentAnswer>()
                .Where(sa => sa.Status == StudentAnswerStatus.StudentAnswer && sa.TestAttempId == testAttemptId)
                .ProjectTo<StudentAnswerDto>().ToListAsync();
        }

        public async Task<List<AnswerDto>> GetCorrectAnswersByQuizId(Guid quizId)
        {
            var query = from ans in _ws.GetAll<Answer>()
                        join qq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == quizId)
                        on ans.QuestionId equals qq.QuestionId
                        select new AnswerDto
                        {
                            Id = ans.Id,
                            IsCorrect = ans.IsCorrect,
                            LAnswer = ans.LAnswer,
                            QuestionId = ans.QuestionId,
                            RAnswer = ans.RAnswer,
                            SequenceOrder = ans.SequenceOrder
                        };
            return await query.ToListAsync();
        }

        public async Task<List<AnswerDto>> GetCorrectAnswersByQuestionId(Guid questionId)
        {
            var query = _ws.GetAll<Answer>().Where(s => s.QuestionId == questionId).Select(ans => new AnswerDto
            {
                Id = ans.Id,
                IsCorrect = ans.IsCorrect,
                LAnswer = ans.LAnswer,
                QuestionId = ans.QuestionId,
                RAnswer = ans.RAnswer,
                SequenceOrder = ans.SequenceOrder
            });
            return await query.ToListAsync();
        }

        public async Task<TestAttemptDto> TeacherTakeScoreOpenEndQuestion(TeacherStudentAnswerDto input)
        {
            var qq = await _ws.GetRepo<QuestionQuiz>().GetAsync(input.QuestionQuizId);
            if (qq == null)
            {
                throw new UserFriendlyException(String.Format("QuestionQuiz id {0} is not exist", input.QuestionQuizId));
            }

            if (input.Mark > qq.Mark)
                throw new UserFriendlyException("Mark can't greater than " + qq.Mark);

            //var studentId = (await _ws.GetRepo<CourseAssignedStudent>().GetAsync(input.CourseAssignedStudentId)).StudentId;
            var item = await _ws.GetAll<StudentAnswer>().Where(s => s.Status == StudentAnswerStatus.StudentAnswer && s.QuestionId == qq.QuestionId && s.TestAttempId == input.TestAttempId).FirstOrDefaultAsync();
            float oldMark = 0;
            if (item == null)
            {
                item = new StudentAnswer
                {
                    Mark = input.Mark,
                    QuestionId = qq.QuestionId,
                    TestAttempId = input.TestAttempId,
                    Status = StudentAnswerStatus.StudentAnswer,
                    //CreatorUserId = studentId
                };
                await _ws.InsertAsync(item);
                //Logger.Debug("TeacherTakeScoreOpenEndQuestion >> vao day " + item.CreatorUserId);
            }
            else
            {
                oldMark = item.Mark.HasValue ? item.Mark.Value : 0;
                item.Mark = (!input.Mark.HasValue || input.Mark < 0) ? 0 : input.Mark;
                await _ws.UpdateAsync(item);
            }

            //update score for test attempt
            var testAttempt = await _ws.GetRepo<TestAttempt>().GetAsync(item.TestAttempId.Value);
            testAttempt.Score += item.Mark - oldMark;
            await _ws.UpdateAsync(testAttempt);


            //update user certification
            CurrentUnitOfWork.SaveChanges();
            var courseInstanceId = (await _ws.GetRepo<QuizSetting>().GetAsync(testAttempt.QuizSettingId)).CourseInstanceId;
            var cer = await this._userCertificationManager.CreateUpdateUserCertification(testAttempt.CourseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateOnly);

            var taDto = ObjectMapper.Map<TestAttemptDto>(testAttempt);
            taDto.UserCertification = cer;

            return taDto;
        }

    }
}
