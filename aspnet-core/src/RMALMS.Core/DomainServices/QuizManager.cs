using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Users;
using RMALMS.Common;
using RMALMS.DomainServices.Entity;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.DomainServices
{
    public class QuizManager : BaseDomainService, IQuizManager
    {
        private IUserCertificationManager _userCertificationManager;

        public QuizManager(IUserCertificationManager userCertificationManager) : base()
        {
            this._userCertificationManager = userCertificationManager;
        }
        public async Task<TestAttempt> ProcessScore(long studentId, Guid testAttemptId, PageType questionType)
        {
            var item = await WorkScope.GetRepo<TestAttempt>().GetAsync(testAttemptId);
            if (item.CreatorUserId.HasValue)
            {
                studentId = item.CreatorUserId.Value;
            }            
       
            item.Status = TestAttemptStatus.Marking;
            item.Score = 0;
            if(questionType == PageType.Survey)
            {
                await WorkScope.UpdateAsync<TestAttempt>(item);
                CurrentUnitOfWork.SaveChanges();
                return item;
            }
            var quizId = await WorkScope.GetAll<QuizSetting>().Where(qs => qs.Id == item.QuizSettingId).Select(qs => qs.QuizId).FirstOrDefaultAsync();
            
            //var questionIds = await WorkScope.GetAll<QuestionQuiz>().Where(qq => qq.QuizId == quizId).Select(x => x.QuestionId).ToListAsync();
            var questionIds = WorkScope.GetAll<StudentAnswer>()
                .Where(s => s.TestAttempId == testAttemptId && s.Status == StudentAnswerStatus.StudentAnswer)
                .Select(s => s.QuestionId)
                .Distinct();

            var questions = await (from qid in questionIds 
                            join qq in WorkScope.GetRepo<QuestionQuiz>()
                            .GetAllIncluding(s => s.Question)
                            .Where(qq => qq.QuizId == quizId)
                            on qid equals qq.QuestionId
                            select new
            {
                qq.QuestionId,
                qq.Question.Type,
                qq.Mark
            }).ToListAsync();

            var questions1 = await (from qq in WorkScope.GetRepo<QuestionQuiz>().GetAllIncluding(s => s.Question).Where(qq => qq.QuizId == quizId)
                                    join qid in questionIds on qq.QuestionId equals qid into qids
                                    where qids.Any()                                   
                                   select new
                                   {
                                       qq.QuestionId,
                                       qq.Question.Type,
                                       qq.Mark
                                   }).ToListAsync();

            //var questions = WorkScope.GetRepo<QuestionQuiz>().GetAllIncluding(s => s.Question).Where(qq => qq.QuizId == quizId).Select(s => new
            //{
            //    s.QuestionId,
            //    s.Question.Type,
            //    s.Mark
            //});

            //var correctAnswers = await WorkScope.GetAll<Answer>().Where(ans => questionIds.Contains(ans.QuestionId)).ToListAsync();
            var correctAnswers = await (from a in WorkScope.GetAll<Answer>()
                                       join qid in questionIds on a.QuestionId equals qid into qs
                                        where qs.Any()
                                        select a).ToListAsync();

            var studentAnswers = await WorkScope.GetAll<StudentAnswer>().Where(sa => sa.TestAttempId == testAttemptId && sa.CreatorUserId == studentId).ToListAsync();
            if (studentAnswers.Count() == 0)
            {
                Logger.Debug("student does not answer any question");
                await WorkScope.UpdateAsync(item);
                //for final quiz -> generate user certificate, completed courseAssignedStudent
                if (questionType == PageType.QuizFinal)
                {
                    CurrentUnitOfWork.SaveChanges();
                    await CompletedCourse(item.CourseAssignedStudentId);
                }
                return item;
            }
            //calculate score for each question
            foreach (var question in questions)
            {
                var studentAnsDto = studentAnswers.Where(x => x.QuestionId == question.QuestionId && x.AnswerId.HasValue).FirstOrDefault();
                if (studentAnsDto == null)
                {
                    Logger.Debug("no answer for questionId => " + question.QuestionId);
                    continue;
                }
                bool isCorrect = true;
                switch (question.Type)
                {
                    case QuestionType.MCQ:
                        var correctAnsIds = correctAnswers.Where(x => x.QuestionId == question.QuestionId && x.IsCorrect.HasValue && x.IsCorrect.Value).OrderBy(s => s.Id).Select(s => s.Id).ToList();
                        var studentAnsIds = studentAnswers.Where(x => x.QuestionId == question.QuestionId && x.AnswerId.HasValue).OrderBy(x => x.AnswerId).Select(s => s.AnswerId.Value).ToList();

                        int count = correctAnsIds.Count();
                        if (count != studentAnsIds.Count())
                        {
                            isCorrect = false;
                        }
                        else
                        {
                            isCorrect = correctAnsIds.Except(studentAnsIds).Count() == 0 && studentAnsIds.Except(correctAnsIds).Count() == 0;
                        }

                        break;
                    case QuestionType.SCQ:
                    case QuestionType.TrueFalse:
                    case QuestionType.FixedWord:
                        var correctAns = correctAnswers.Where(x => x.QuestionId == question.QuestionId && x.IsCorrect.HasValue && x.IsCorrect.Value).FirstOrDefault();
                        isCorrect = correctAns.Id == studentAnsDto.AnswerId;
                        break;
                    case QuestionType.Matching:
                        var correctMAnswers = correctAnswers
                            .Where(x => x.QuestionId == question.QuestionId && x.LAnswer != null)
                            .OrderBy(s => s.Id)
                            .Select(s => new { s.Id, s.LAnswer })
                            .ToList();
                        var studentMAnswers = studentAnswers
                            .Where(x => x.QuestionId == question.QuestionId && x.AnswerId.HasValue && x.AnswerText != null)
                            .OrderBy(x => x.AnswerId)
                            .Select(x => new { Id = x.AnswerId, x.AnswerText })
                            .ToList();
                        if(correctMAnswers.Count() == studentMAnswers.Count()){
                            isCorrect = !(from ca in correctMAnswers
                                          join sa in studentMAnswers on ca.Id equals sa.Id
                                          select new { ca.LAnswer, sa.AnswerText }
                                     ).Any(s => s.LAnswer != s.AnswerText);
                        }
                        else
                        {
                            isCorrect = false;
                        }
                        break;
                    case QuestionType.Ranked:
                        var correctRAnswers = correctAnswers.Where(x => x.QuestionId == question.QuestionId && x.SequenceOrder.HasValue).OrderBy(s => s.Id).Select(s => new { s.Id, s.SequenceOrder }).ToList();
                        var studentRAnswers = studentAnswers.Where(x => x.QuestionId == question.QuestionId && x.AnswerId.HasValue && x.AnswerText != null).OrderBy(x => x.AnswerId).Select(x => new { AnswerId = x.AnswerId.Value, x.AnswerText }).ToList();
                        isCorrect = !(from ca in correctRAnswers
                                      join sa in studentRAnswers on ca.Id equals sa.AnswerId
                                      select new { ca.SequenceOrder, sa.AnswerText }
                                     ).Any(s => s.SequenceOrder.ToString() != s.AnswerText);

                        break;
                    case QuestionType.OpenEnd:
                        break;
                    case QuestionType.MatrixTableQuestion:
                        break;
                    default:
                        break;
                }
                studentAnsDto.Mark = isCorrect ? question.Mark : 0;
                item.Score += studentAnsDto.Mark;
                await WorkScope.UpdateAsync(studentAnsDto);
            }
            await WorkScope.UpdateAsync(item);
            //var itemDto = ObjectMapper.Map<TestAttemptDto>(item);
            
            //for final quiz -> generate user certificate, completed courseAssignedStudent
            if (questionType == PageType.QuizFinal)
            {
                CurrentUnitOfWork.SaveChanges();                
                await CompletedCourse(item.CourseAssignedStudentId);
            }
            return item;
        }

        public async Task CompletedCourse(Guid courseAssignedStudentId)
        {
            var courseAssignedStudent = await WorkScope.GetRepo<CourseAssignedStudent>()
                .GetAllIncluding( s => s.CourseInstance, s => s.CourseInstance.Course)
                .Where(s => s.Id == courseAssignedStudentId)                
                .FirstOrDefaultAsync();
            //var courseAssignedStudent = await WorkScope.GetRepo<CourseAssignedStudent>().GetAsync(courseAssignedStudentId);            
            if (courseAssignedStudent != null)
            {
                courseAssignedStudent.Status = AssignedStatus.Completed;
                await WorkScope.UpdateAsync(courseAssignedStudent);
            }
            var courseInstanceId = courseAssignedStudent.CourseInstanceId;            
            var studentId = courseAssignedStudent.StudentId;
            var courseSourse = courseAssignedStudent.CourseInstance.Course.Sourse;
            UserCertification cer = null;
            if (courseSourse == CourseSource.RMA)
            {
                cer = await _userCertificationManager.CreateUpdateUserCertification(courseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }
            else
            {   
                cer = await _userCertificationManager.CreateUpdateUserCertificationScorm(courseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }

            //update student level
            var studentLevel = await WorkScope.GetRepo<User, long>().GetAllIncluding(s => s.Status)
                .Where(s => s.Id == courseAssignedStudent.StudentId).Select( s => new {User = s, s.Status })
                .FirstOrDefaultAsync();

            if (studentLevel!= null && studentLevel.Status != null)
            {
                await CurrentUnitOfWork.SaveChangesAsync();
                var courseLevelCount = await (from c in WorkScope.GetAll<Course>()
                                              join cl in WorkScope.GetAll<CourseLevel>().Where(s => s.Level == studentLevel.Status.Level) on c.LevelId equals cl.Id
                                              join cc in
                                                  from ci in WorkScope.GetAll<CourseInstance>()
                                                  join cas in WorkScope.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == studentId && s.Status == AssignedStatus.Completed)
                                                  on ci.Id equals cas.CourseInstanceId
                                                  select ci.CourseId
                                              on c.Id equals cc
                                              select c.Id
                            ).Distinct().CountAsync();

                if ((studentLevel.Status.LowCompareOperation == CompareOperation.GreaterThan && courseLevelCount > studentLevel.Status.RequiredNumber)
                    || (studentLevel.Status.LowCompareOperation == CompareOperation.GreaterEqual && courseLevelCount >= studentLevel.Status.RequiredNumber))
                {
                    //studentLevel.s.StatusId = 
                    var orderedLevels = await WorkScope.GetAll<UserStatus>().OrderBy(s => s.Level).Select(s => new { s.Level, s.Id }).ToListAsync();
                    //small -> big by level
                    foreach (var level in orderedLevels)
                    {
                        if (level.Level > studentLevel.Status.Level)
                        {
                            studentLevel.User.StatusId = level.Id;
                            await WorkScope.UpdateAsync<User, long>(studentLevel.User);
                            break;
                        }
                    }
                }
            }
                        
        }
    }
}
