using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.DomainServices;
using RMALMS.DomainServices.Entity;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Questions.Dto;
using RMALMS.Quizzes.Dto;
using RMALMS.TestAttempts.Dto;
using RMALMS.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Quizzes
{
    [AbpAuthorize]
    public class QuizAppService : AsyncCrudAppService<Quiz, QuizDto, Guid, PagedResultRequestDto, CreateQuizDto, QuizDto>, IQuizAppService
    {
        private readonly IWorkScope _ws;
        private readonly ICourseManager _courseManager;
        private readonly IQuizManager _quizManager;

        public QuizAppService(IRepository<Quiz, Guid> repository, IWorkScope workScope, ICourseManager courseManager, IQuizManager quizManager) : base(repository)
        {
            _ws = workScope;
            this._courseManager = courseManager;
            this._quizManager = quizManager;


        }


        [HttpPost]
        public async Task<GridResult<QuizDto>> GetQuizzesByCourseIdPagging(GridQuizzesParam input)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == input.courseId).ProjectTo<QuizDto>();
            return await query.GetGridResult(query, input.input);
        }
        public async Task<List<QuizDto>> GetQuizzesByCourseId(Guid courseId)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == courseId).ProjectTo<QuizDto>();
            return await query.ToListAsync();
        }

        public async Task<List<SelectQuizDto>> GetSelectQuizzesByCourseInstanceId(Guid courseInstanceId)
        {
            var query = _ws.GetRepo<QuizSetting>().GetAllIncluding(s => s.Quiz).Where(p => p.CourseInstanceId == courseInstanceId && p.Quiz.Status == QuizStatus.Published)
                .Select(s => new SelectQuizDto { Id = s.Id, Title = s.Quiz.Title, Type = s.Quiz.Type });
            return await query.ToListAsync();
        }

        public async override Task<QuizDto> Update(QuizDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            List<string> notifymessage = new List<string>();
            if (item.Title != input.Title)
            {
                notifymessage.Add("<p>Title has changed from '" + item.Title + "' to '" + input.Title + "' </p>");
            }
            if (item.Content != input.Content)
            {
                notifymessage.Add("<p>Content has changed </p>");
            }
            if (item.Type != input.Type)
            {
                notifymessage.Add("<p>Type has changed from '" + (QuizType)item.Type + "' to '" + (QuizType)input.Type + "'</p>");
            }
            //if (item.settings.Point != input.settings.Point)
            //{
            //    notifymessage.add("<p>score has changed from '" + (item.point != null ? item.point.tostring() : "none set") + "' to '" + (input.point != null ? input.point.tostring() : "none set") + "'</p>");
            //}
            if (item.IsShuffleAnswer != input.IsShuffleAnswer)
            {
                notifymessage.Add("<p>Shuffle answer has changed to '" + input.IsShuffleAnswer.ToString() + "'</p>");
            }
            if (item.ScoreKeepType != input.ScoreKeepType)
            {
                notifymessage.Add("<p>Quiz score to keep type has changed from '" + (QuizScoreToKeepType)item.ScoreKeepType + "' to '" + (QuizScoreToKeepType)input.ScoreKeepType + "' </p>");
            }
            if (item.AllowAttempts != input.AllowAttempts)
            {
                notifymessage.Add("<p>Allow attempts has changed from '" + (item.AllowAttempts != null ? item.AllowAttempts.ToString() : "none set") + "' to '" + (input.AllowAttempts != null ? input.AllowAttempts.ToString() : "none set") + "'</p>");
            }
            if (item.LookQuestionAfterAnswer != input.LookQuestionAfterAnswer)
            {
                notifymessage.Add("<p>Look Question After Answer has changed to '" + input.LookQuestionAfterAnswer.ToString() + "'</p>");
            }
            if (item.ResponseType != input.ResponseType)
            {
                notifymessage.Add("<p>Response Type has changed from '" + (StudentReponseType)item.ResponseType + "' to '" + (StudentReponseType)input.ResponseType + "'</p>");
            }
            MapToEntity(input, item);
            await Repository.UpdateAsync(item);
            var itemDto = MapToEntityDto(item);
            var setting = _ws.GetAll<QuizSetting, Guid>().Where(s => s.Id == input.settings.Id).FirstOrDefault();
            if (setting == null)
            {
                notifymessage.Add("<p>Setting has been set</p>");
                setting = ObjectMapper.Map<QuizSetting>(input.settings);
                setting.QuizId = item.Id;
                setting.Id = await _ws.InsertAndGetIdAsync(setting);
            }
            else
            //if (setting.NoOfDueDays != input.settings.NoOfDueDays || setting.StartTimeUtc != input.settings.StartTimeUtc || setting.EndTimeUtc != input.settings.EndTimeUtc)
            {
                notifymessage.Add("<p>Setting has been changed</p>");
                ObjectMapper.Map<QuizSettingsDto, QuizSetting>(input.settings, setting);
                //setting.NoOfDueDays = input.settings.NoOfDueDays;
                //setting.StartTimeUtc = input.settings.StartTimeUtc;
                //setting.EndTimeUtc = input.settings.EndTimeUtc;
                setting = await _ws.UpdateAsync(setting);
            }

            var oldgroupassignquiz = (from gaq in _ws.GetAll<GroupAssingedQuiz, Guid>()
                                      where gaq.QuizSettingId == setting.Id
                                      select gaq.CourseGroupId).ToList();
            var newgroupassign = new List<Guid>();
            if (input.GroupsAssingedQuiz != null)
                newgroupassign = input.GroupsAssingedQuiz.FindAll(n => !oldgroupassignquiz.Contains(n));
            foreach (var group in newgroupassign)
            {
                var groupasign = new GroupAssingedQuiz
                {
                    CourseGroupId = group,
                    QuizSettingId = setting.Id
                };
                await _ws.InsertAndGetIdAsync(groupasign);
            }
            var deleteitem = oldgroupassignquiz.FindAll(o => !input.GroupsAssingedQuiz.Contains(o));
            await _ws.GetRepo<GroupAssingedQuiz>().DeleteAsync(ga => (ga.QuizSettingId == input.settings.Id && deleteitem.Contains(ga.CourseGroupId)));
            if (newgroupassign.Count > 0 || deleteitem.Count > 0)
                notifymessage.Add("<p>Assigned group has been changed</p>");
            if (input.AllowNotify && notifymessage.Count > 0)
            {
                var notify = new Annoucement();
                notify.Title = "Update quiz '" + item.Title + "'";
                notify.Content = string.Join("\n", notifymessage);
                notify.CourseInstanceId = input.CourseInstanceId;
                await _ws.InsertAndGetIdAsync(notify);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            itemDto.settings = _ws.GetAll<QuizSetting, Guid>().Where(s => s.Id == setting.Id).ProjectTo<QuizSettingsDto>().FirstOrDefault();

            return itemDto;
        }


        public async override Task<QuizDto> Create(CreateQuizDto input)
        {
            var isExistCourse = await _ws.GetAll<Course, Guid>().AnyAsync(c => c.Id == input.CourseId);
            if (!isExistCourse)
                throw new UserFriendlyException(string.Format("The course id {0} is not exist", input.CourseId));
            var item = ObjectMapper.Map<Quiz>(input);
            item.Id = await _ws.InsertAndGetIdAsync(item);
            var itemDto = MapToEntityDto(item);
            var setting = ObjectMapper.Map<QuizSetting>(input.settings);
            setting.QuizId = item.Id;
            setting.Id = await _ws.InsertAndGetIdAsync(setting);
            foreach (var groupId in input.GroupsAssingedQuiz)
            {
                var groupasign = new GroupAssingedQuiz();
                groupasign.CourseGroupId = groupId;
                groupasign.QuizSettingId = setting.Id;
                await _ws.InsertAndGetIdAsync(groupasign);
            }
            if (input.AllowNotify)
            {
                var notify = new Annoucement();
                notify.Title = "Create quiz '" + item.Title + "'";
                notify.Content = "New quiz has been added";
                notify.CourseInstanceId = input.CourseInstanceId;
                await _ws.InsertAndGetIdAsync(notify);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            itemDto.settings = _ws.GetAll<QuizSetting, Guid>().Where(s => s.Id == setting.Id).ProjectTo<QuizSettingsDto>().FirstOrDefault();
            return itemDto;
        }

        public async override Task<QuizDto> Get(EntityDto<Guid> input)
        {
            try
            {
                var item = await Repository.GetAsync(input.Id);
                var itemDto = MapToEntityDto(item);
                var courseintance = _ws.GetRepo<CourseInstance>().GetAll().Where(ci => ci.CourseId == item.CourseId && ci.Status == CourseSettingStatus.Active).FirstOrDefault();
                if (courseintance != null)
                {
                    itemDto.CourseInstanceId = courseintance.Id;
                    var setting = _ws.GetAll<QuizSetting, Guid>().Where(a => a.QuizId == item.Id && a.CourseInstanceId == courseintance.Id).ProjectTo<QuizSettingsDto>().FirstOrDefault();
                    if (setting == null)
                        setting = new QuizSettingsDto();
                    itemDto.settings = setting;
                }
                return itemDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(string.Format("Data not valid"));

            }
        }
        public async Task<QuizDto> GetQuizDataById(Guid quizId, Guid courseInstanceId)
        {
            try
            {
                var item = await Repository.GetAsync(quizId);
                var itemDto = MapToEntityDto(item);

                itemDto.CourseInstanceId = courseInstanceId;
                var setting = _ws.GetAll<QuizSetting, Guid>().Where(a => a.QuizId == item.Id && a.CourseInstanceId == courseInstanceId).ProjectTo<QuizSettingsDto>().FirstOrDefault();
                if (setting == null)
                    setting = new QuizSettingsDto();
                itemDto.settings = setting;
                var listIds = _ws.GetAll<GroupAssingedQuiz, Guid>().Where(g => g.QuizSettingId == setting.Id).Select(g => g.CourseGroupId).ToList();
                itemDto.GroupsAssingedQuiz = listIds;
                return itemDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(string.Format("Data not valid"));

            }
        }

        public async Task<QuizOptionDto> GetQuizOptions(Guid quizId, Guid courseInstanceId)
        {
            var item = await Repository.GetAsync(quizId);
            var itemDto = ObjectMapper.Map<QuizOptionDto>(item);
            var setting = _ws.GetAll<QuizSetting>().Where(a => a.QuizId == quizId && a.CourseInstanceId == courseInstanceId).ProjectTo<QuizSettingsDto>().FirstOrDefault();
            if (setting == null)
                setting = new QuizSettingsDto();
            itemDto.settings = setting;
            return itemDto;

        }

        public async Task<QuizOptionDto> GetQuizOptionsAndTestAttemps(Guid quizSettingId, Guid courseInstanceId, string quizType)
        {
            var setting = await _ws.GetRepo<QuizSetting>().GetAsync(quizSettingId);

            var courseSetting = await _ws.GetRepo<CourseInstance>().GetAsync(courseInstanceId);

            if (setting.ApplySameStartEndTimeAsCourse)
            {
                setting.StartTimeUtc = courseSetting.StartTime;
                setting.EndTimeUtc = courseSetting.EndTime;
            }

            if (setting != null && (setting.StartTimeUtc ?? DateTime.MinValue) < DateTimeUtils.GetNow() && (setting.EndTimeUtc ?? DateTime.MaxValue) > DateTimeUtils.GetNow())
            {
                var quiz = await Repository.GetAsync(setting.QuizId);
                var quizDto = ObjectMapper.Map<QuizOptionDto>(quiz);

                quizDto.settings = ObjectMapper.Map<QuizSettingsDto>(setting);
                var studentId = AbpSession.UserId.Value;
                var qcourseAssignedStudent = _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == studentId && s.CourseInstanceId == setting.CourseInstanceId);
                var courseAssignedStudent = await qcourseAssignedStudent.LastOrDefaultAsync();
                if (courseAssignedStudent == null || (courseAssignedStudent.Status != AssignedStatus.Accepted && courseAssignedStudent.Status != AssignedStatus.Completed))
                {
                    throw new UserFriendlyException(String.Format("Student id {0} has not accepted to do quiz in this course", studentId));
                }

                //testAttempts
                var testAttempts = _ws.GetAll<TestAttempt>().Where(ta => ta.CourseAssignedStudentId == courseAssignedStudent.Id && ta.QuizSettingId == setting.Id);

                var lastTestingAttempt = await testAttempts.Where(t => t.Status == TestAttemptStatus.Testing).LastOrDefaultAsync();
                if (setting.NoOfDueDays > 0 && lastTestingAttempt != null && lastTestingAttempt.CreationTime.AddDays((Double)setting.NoOfDueDays) < DateTimeUtils.GetNow())
                {
                    await _quizManager.ProcessScore(studentId, lastTestingAttempt.Id, quizType == "quiz_final" ? PageType.QuizFinal : PageType.Quiz);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    testAttempts = _ws.GetAll<TestAttempt>().Where(ta => ta.CourseAssignedStudentId == courseAssignedStudent.Id && ta.QuizSettingId == setting.Id);
                }
                quizDto.TestAttempts = await testAttempts.ProjectTo<TestAttemptDto>().ToListAsync();

                //testing attempt
                var allowAttemptCount = quiz.AllowAttempts == null ? 1 : quiz.AllowAttempts;
                var testedAttempCount = testAttempts.Count(s => s.Status == TestAttemptStatus.Marking);
                if (testedAttempCount < allowAttemptCount)
                {
                    var testingAttempt = await testAttempts.Where(ta => ta.Status == TestAttemptStatus.Testing).LastOrDefaultAsync();


                    if (testingAttempt != null)
                    {

                        var testingAttemptDto = ObjectMapper.Map<TestAttemptDto>(testingAttempt);
                        //questions
                        var qselectedQuestionIds = _ws.GetAll<StudentAnswer>()
                            .Where(s => s.TestAttempId == testingAttemptDto.Id)
                            .Select(s => s.QuestionId).Distinct();

                        var qquestions =
                            from qid in qselectedQuestionIds
                            join a in _ws.GetAll<Answer>() on qid equals a.QuestionId into answers
                            join qq in _ws.GetAll<QuestionQuiz>().Where(s => s.QuizId == setting.QuizId) on qid equals qq.QuestionId
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

                        testingAttemptDto.Questions = await qquestions.ToListAsync();

                        //student answers
                        testingAttemptDto.StudentAnswers = await _ws.GetAll<StudentAnswer>()
                            .Where(sa => sa.Status == StudentAnswerStatus.StudentAnswer && sa.CreatorUserId == studentId && sa.TestAttempId == testingAttemptDto.Id)
                            .ProjectTo<StudentAnswerDto>().ToListAsync();

                        if (quiz.TimeLimit.HasValue && quiz.TimeLimit > 0)
                        {
                            var timeLimit = await _ws.GetRepo<QuizSetting>().GetAllIncluding(qs => qs.Quiz).Where(qs => qs.Id == setting.Id && qs.Quiz.TimeLimit.HasValue).Select(qs => qs.Quiz.TimeLimit).FirstOrDefaultAsync();
                            if (timeLimit.HasValue && timeLimit > 0)
                            {
                                int timePass = (int)DateTimeUtils.GetNow().Subtract(testingAttempt.CreationTime).TotalSeconds;
                                testingAttemptDto.TimeRemaining = timeLimit * 60 - timePass;
                            }
                        }

                        quizDto.TestingAttempt = testingAttemptDto;
                        //}
                    }
                }

                return quizDto;
            }
            else
            {
                var result = new QuizOptionDto();
                result.IsExpired = true;
                result.settings = ObjectMapper.Map<QuizSettingsDto>(setting);
                return result;
            }
        }

        public async Task<List<StudentQuizAssignmentScore>> GetStudentQuizProgress(Guid courseInstanceId)
        {
            var userId = AbpSession.UserId.Value;
            var courseAssignedStudentId = (await _ws.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == userId).LastOrDefaultAsync()).Id;
            return await _courseManager.GetStudentQuizScores(courseAssignedStudentId, courseInstanceId);

        }
    }
}
