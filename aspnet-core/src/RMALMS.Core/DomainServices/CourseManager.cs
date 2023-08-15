using Abp.Domain.Services;
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
    public class CourseManager : BaseDomainService, ICourseManager
    {
        public IQueryable<CourseAssignedStudent> GetStudentAssignedCourseByStatus(Guid courseSettingId, AssignedStatus? Status, bool includingUser = true)
        {
            IQueryable<CourseAssignedStudent> query = null;
            if (includingUser)
            {
                query = WorkScope.GetRepo<CourseAssignedStudent>().GetAllIncluding(s => s.Student);
            }
            else
            {
                query = WorkScope.GetAll<CourseAssignedStudent>();
            }
            if (Status.HasValue)
            {
                query = query.Where(s => s.Status == Status);
            }
            return query.Where(s => s.CourseInstanceId == courseSettingId);
        }

        public IQueryable<CourseGroup> GetCourseGroupsByCourse(Guid courseIntanceId, bool includeStudent = false)
        {
            IQueryable<CourseGroup> query = null;
            if (includeStudent)
            {
                query = WorkScope.GetRepo<CourseGroup>().GetAllIncluding(s => s.StudentCourseGroups);
            }
            else
            {
                query = WorkScope.GetAll<CourseGroup>();
            }
            return query.Where(s => s.CourseInstanceId == courseIntanceId);
        }

        public IQueryable<User> GetAllStudentAssignedToCourse(Guid courseInstanceId)
        {
            var alreadyAssingedStudentId = WorkScope.GetAll<CourseAssignedStudent>()
                                                    .Where(s => s.CourseInstanceId == courseInstanceId
                                                             && s.Status != AssignedStatus.Rejected)
                                                    .Select(s => s.StudentId);

            // get students are assigned via group
            var assignedGroupIds =
                from g in WorkScope.GetAll<GroupAssignedCourse>().Where(s => s.CourseInstanceId == courseInstanceId)
                select g.GroupId;

            // ignore user already assigned to system
            var assignStudentIds =
                from ug in WorkScope.GetAll<UserGroup>()
                join g in assignedGroupIds on ug.GroupId equals g into groups
                where groups.Any()
                select ug.UserId;

            var qusers =
                from user in WorkScope.GetAll<User, long>()
                join au in assignStudentIds on user.Id equals au into students
                join inu in alreadyAssingedStudentId on user.Id equals inu into invitedstudents
                where students.Any() || invitedstudents.Any()
                select user;

            return qusers;
        }

        public async Task CreateCourseInstance(Guid courseId)
        {
            //create course setting (course instant)
            CourseInstance instance = new CourseInstance()
            {
                CourseId = courseId,
                Status = CourseSettingStatus.Active
            };
            instance.Id = await WorkScope.InsertAndGetIdAsync<CourseInstance>(instance);
            await CurrentUnitOfWork.SaveChangesAsync();

            //create course group - everyone
            var cg = new CourseGroup
            {
                CourseInstanceId = instance.Id,
                Name = Const.EVERYONE_COURSE_GROUP,
                IsEveryOne = true
            };
            await WorkScope.InsertAsync<CourseGroup>(cg);
        }

        public IQueryable<CourseAssignedStatus> GetAvailableCoursesForStudent(long userId)
        {
            var query =
                from ci in WorkScope.GetRepo<CourseInstance>().GetAllIncluding(ci => ci.Course).Where(s => s.Course.State == CourseState.Publish)
                join cas in WorkScope.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == userId) on ci.Id equals cas.CourseInstanceId into statuses
                join g in
                    from gac in WorkScope.GetAll<GroupAssignedCourse>()
                    join g in WorkScope.GetAll<Group>() on gac.GroupId equals g.Id
                    join ug in WorkScope.GetAll<UserGroup>().Where(s => s.UserId == userId) on g.Id equals ug.GroupId
                    select gac.CourseInstanceId
                on ci.Id equals g into groups
                from status in statuses.DefaultIfEmpty().Take(1)
                where statuses.Any() || groups.Any()
                select new CourseAssignedStatus
                {
                    CourseInstance = ci,
                    AssignedStatus = status == null ? (AssignedStatus?)null : status.Status
                };
            return query;

        }


        public IQueryable<StudentAssignedQuizAssignment> GetStudentAssignedQuizzes(Guid courseInstanceId)
        {

            var _ws = WorkScope;
            var qassignedstudents = GetCourseAssignedStudentsAcceptedAndCompleted(courseInstanceId);
            //[{quizSettingId, Point, PageId}]
            var qquizPageLinks =
                from q in _ws.GetAll<QuizSetting>().Where(s => s.CourseInstanceId == courseInstanceId)
                join pl in _ws.GetAll<PageLinkExam>().Where(s => s.LinkType != "survey") on q.Id equals pl.LinkId
                select new { QuizSettingId = q.Id, Point = q.Point ?? 0, pl.PageId };

            //[{courseGroupId, QuizSettingId, IsEveryOne}]
            var qGroupAssignWithSettings =
                from q in _ws.GetAll<GroupAssingedQuiz>()
                join s in _ws.GetAll<QuizSetting>().Where(s => s.CourseInstanceId == courseInstanceId) on q.QuizSettingId equals s.Id
                join cg in _ws.GetAll<CourseGroup>() on q.CourseGroupId equals cg.Id
                select new { q.CourseGroupId, QuizSettingId = s.Id, cg.IsEveryOne };

            //student assigned quiz
            //[{QuizSettingId, Point, CourseGroupId, PageId}] for Course Group
            var qQuizCourseGroups =
                from quiz in qquizPageLinks
                join cq in qGroupAssignWithSettings.Where(s => !s.IsEveryOne) on quiz.QuizSettingId equals cq.QuizSettingId
                select new { quiz.QuizSettingId, quiz.Point, cq.CourseGroupId, quiz.PageId };

            //[{QuizSettingId, Point, CourseGroupId, PageId}] for Course Group Everyone
            var quizEveveryOne =
                from quiz in qquizPageLinks
                join cq in qGroupAssignWithSettings.Where(s => s.IsEveryOne) on quiz.QuizSettingId equals cq.QuizSettingId
                select new { quiz.QuizSettingId, quiz.Point, cq.CourseGroupId, quiz.PageId };

            //[{StudentId, Point, QuizSettingId, PageId}]
            var qStudentQuiz =
                from s in qassignedstudents
                join q in _ws.GetAll<StudentCourseGroup>() on s.Id equals q.AssignedStudentId
                join qc in qQuizCourseGroups on q.CourseGroupId equals qc.CourseGroupId
                select new {CourseAssignedStudentId = s.Id, s.StudentId, qc.Point, qc.QuizSettingId, qc.PageId };

            var qStudentQuizGroupByPage =
                from q in qStudentQuiz
                group q by new { q.CourseAssignedStudentId, q.PageId } into g
                select new { g.Key.CourseAssignedStudentId, Quiz = g.Select(s => new SettingScore { SettingId = s.QuizSettingId, Point = s.Point, PageId = s.PageId }).FirstOrDefault() };

            //[{StudentId, Point, QuizSettingId, PageId}] for Everyone
            var qStudentQuizEveryone =
                from s in qassignedstudents
                from q in quizEveveryOne
                select new { CourseAssignedStudentId = s.Id, q.Point, q.QuizSettingId, q.PageId };

            var qStudentQuizEveryoneGroupByPage =
               from q in qStudentQuizEveryone
               group q by new { q.CourseAssignedStudentId, q.PageId } into g
               select new { g.Key.CourseAssignedStudentId, Quiz = g.Select(s => new SettingScore { SettingId = s.QuizSettingId, Point = s.Point, PageId = s.PageId }).FirstOrDefault() };

            var qstudentTakeQuizs =
                from q in qassignedstudents
                join sq in qStudentQuizGroupByPage on q.Id equals sq.CourseAssignedStudentId into Quizs
                join sqe in qStudentQuizEveryoneGroupByPage on q.Id equals sqe.CourseAssignedStudentId into QuizsEverone
                select new StudentAssignedQuizAssignment
                {
                    CourseAssignedStudentId = q.Id,
                    StudentId = q.StudentId,
                    AssignedByQuizGroups = Quizs.Select(s => s.Quiz).ToList(),
                    EveryOneAssignmentQuizzes = QuizsEverone.Select(s => s.Quiz).ToList()
                };

            return qstudentTakeQuizs;
        }

        public async Task<List<StudentQuizAssignmentScore>> GetStudentQuizScores(Guid assignedStudentId, Guid courseInstanceId)
        {

            var _ws = WorkScope;
            var assignedQuizzes = await GetStudentAssignedQuizzes(courseInstanceId).Where(s => s.CourseAssignedStudentId == assignedStudentId).FirstOrDefaultAsync();
            if (assignedQuizzes == null || assignedQuizzes.AssignedList == null)
            {
                return new List<StudentQuizAssignmentScore>();
            }

            //student quiz score
            var studentTestAttempts = await (from ta in _ws.GetRepo<TestAttempt>().GetAllIncluding(s => s.QuizSetting, s => s.QuizSetting.Quiz).Where(s => s.QuizSetting.CourseInstanceId == courseInstanceId && s.Status == TestAttemptStatus.Marking && s.QuizSetting.Quiz.Type == QuizType.Quiz)
                                             where ta.CourseAssignedStudentId == assignedStudentId
                                             select new
                                             {
                                                 ta.Score,
                                                 ta.QuizSettingId,
                                                 ta.QuizSetting.Quiz.ScoreKeepType,
                                                 ta.MaxScore
                                             }).ToListAsync();

            var results = new List<StudentQuizAssignmentScore>();
            foreach (var quiz in assignedQuizzes.AssignedList)
            {
                var attempts = studentTestAttempts.Where(s => s.QuizSettingId == quiz.SettingId).ToList();
                float studentPoint = 0;
                if (attempts != null && attempts.Count() > 0)
                {
                    if (attempts[0].ScoreKeepType == QuizScoreToKeepType.Highest)
                    {
                        //studentPoint = attempts.Max(s => s.Score.Value);
                        studentPoint = attempts.Max(s => s.MaxScore.HasValue && s.MaxScore.Value > 0 ?  s.Score.Value/s.MaxScore.Value : 0);
                    }
                    else
                    {
                        studentPoint = attempts.Average(s => s.MaxScore.HasValue && s.MaxScore.Value > 0 ? s.Score.Value / s.MaxScore.Value : 0);
                    }
                }
                var sqss = new StudentQuizAssignmentScore
                {
                    TotalScore = quiz.Point.HasValue ? quiz.Point.Value : 0,
                    SettingId = quiz.SettingId,
                    CourseAssignedStudentId = assignedStudentId,
                    StudentScore = studentPoint * (quiz.Point.HasValue ? quiz.Point.Value : 0),
                };
                results.Add(sqss);
            }
            return results;
        }

        public IQueryable<StudentAssignedQuizAssignment> GetStudentAssignedAssignments(Guid courseInstanceId)
        {
            var _ws = WorkScope;
            var qassignedstudents = GetCourseAssignedStudentsAcceptedAndCompleted(courseInstanceId);
            //[{AssignmentSettingId, Point, PageId}]
            var qassignmentPageLinks =
                from q in _ws.GetAll<AssignmentSetting>().Where(s => s.CourseInstanceId == courseInstanceId)
                join pl in _ws.GetAll<PageLinkExam>() on q.Id equals pl.LinkId
                select new { AssignmentSettingId = q.Id, Point = q.Point ?? 0, pl.PageId };

            //[{courseGroupId, AssignmentSettingId, IsEveryOne}]
            var qgroupAssignments =
                from gaa in _ws.GetAll<GroupAssingedAssignment>()
                join s in _ws.GetAll<AssignmentSetting>().Where(s => s.CourseInstanceId == courseInstanceId) on gaa.AssignmentSettingId equals s.Id
                join cg in _ws.GetAll<CourseGroup>() on gaa.CourseGroupId equals cg.Id
                select new { gaa.CourseGroupId, AssignmentSettingId = s.Id, cg.IsEveryOne };

            //student assigned quiz
            //[{AssignmentSettingId, Point, CourseGroupId, PageId}] for Course Group
            var qassignmentCourseGroups =
                from apl in qassignmentPageLinks
                join ga in qgroupAssignments.Where(s => !s.IsEveryOne) on apl.AssignmentSettingId equals ga.AssignmentSettingId
                select new { apl.AssignmentSettingId, apl.Point, ga.CourseGroupId, apl.PageId };

            //[{QuizSettingId, Point, CourseGroupId, PageId}] for Course Group Everyone
            var qassignmentEveveryOne =
                from apl in qassignmentPageLinks
                join ga in qgroupAssignments.Where(s => s.IsEveryOne) on apl.AssignmentSettingId equals ga.AssignmentSettingId
                select new { apl.AssignmentSettingId, apl.Point, ga.CourseGroupId, apl.PageId };

            //[{StudentId, Point, QuizSettingId, PageId}]
            var qStudentAssignments =
                from s in qassignedstudents
                join q in _ws.GetAll<StudentCourseGroup>() on s.Id equals q.AssignedStudentId
                join qc in qassignmentCourseGroups on q.CourseGroupId equals qc.CourseGroupId
                select new { CourseAssignedStudentId = s.Id, qc.Point, qc.AssignmentSettingId, qc.PageId };

            var qStudentAssignmentGroupByPage =
                from q in qStudentAssignments
                group q by new { q.CourseAssignedStudentId, q.PageId } into g
                select new { g.Key.CourseAssignedStudentId, Assignment = g.Select(s => new SettingScore { SettingId = s.AssignmentSettingId, Point = s.Point, PageId = s.PageId }).FirstOrDefault() };

            //[{StudentId, Point, QuizSettingId, PageId}] for Everyone
            var qStudentAssignmentEveryone =
                from s in qassignedstudents
                from q in qassignmentEveveryOne
                select new { CourseAssignedStudentId = s.Id, q.Point, q.AssignmentSettingId, q.PageId };

            var qStudentAssignmentEveryoneGroupByPage =
               from q in qStudentAssignmentEveryone
               group q by new { q.CourseAssignedStudentId, q.PageId } into g
               select new { g.Key.CourseAssignedStudentId, Assignment = g.Select(s => new SettingScore { SettingId = s.AssignmentSettingId, Point = s.Point, PageId = s.PageId }).FirstOrDefault() };

            var qstudentTakeAssignments =
                from q in qassignedstudents
                join sq in qStudentAssignmentGroupByPage on q.Id equals sq.CourseAssignedStudentId into Assignments
                join sq in qStudentAssignmentEveryoneGroupByPage on q.Id equals sq.CourseAssignedStudentId into AssignmentsEverone
                select new StudentAssignedQuizAssignment
                {
                    CourseAssignedStudentId = q.Id,
                    StudentId = q.StudentId,
                    AssignedByQuizGroups = Assignments.Select(s => s.Assignment).ToList(),
                    EveryOneAssignmentQuizzes = AssignmentsEverone.Select(s => s.Assignment).ToList()
                };

            return qstudentTakeAssignments;

        }

        public async Task<List<StudentQuizAssignmentScore>> GetStudentAssignmentScores(Guid assignedStudentId, Guid courseInstanceId)
        {
            var _ws = WorkScope;
            var assignedAssignments = await GetStudentAssignedAssignments(courseInstanceId).Where(s => s.CourseAssignedStudentId == assignedStudentId).FirstOrDefaultAsync();
            if (assignedAssignments == null || assignedAssignments.AssignedList == null)
            {
                return new List<StudentQuizAssignmentScore>();
            }
            //student assignment score           
            var studentAssignmentScores = await _ws.GetRepo<StudentAssignment>().GetAllIncluding(s => s.Assignment)
                .Where(s => s.CourseAssignedStudentId == assignedStudentId && s.Assignment.CourseInstanceId == courseInstanceId)
                .Select(s => new { s.AssignmentSettingId, s.Point }).ToListAsync();

            var results = new List<StudentQuizAssignmentScore>();
            foreach (var assign in assignedAssignments.AssignedList)
            {
                var assignment = studentAssignmentScores.Where(s => s.AssignmentSettingId == assign.SettingId).FirstOrDefault();
                float studentPoint = (assignment == null ? 0 : assignment.Point ?? 0);

                var sqss = new StudentQuizAssignmentScore
                {
                    TotalScore = assign.Point.HasValue ? assign.Point.Value : 0,
                    SettingId = assign.SettingId,
                    CourseAssignedStudentId = assignedStudentId,
                    StudentScore = studentPoint
                };
                results.Add(sqss);
            }
            return results;
        }

        public async Task<StudentScore> GetStudentScore(Guid assignedStudentId, Guid courseInstanceId)
        {
            var quizzes = await GetStudentQuizScores(assignedStudentId, courseInstanceId);
            var assignments = await GetStudentAssignmentScores(assignedStudentId, courseInstanceId);
            var result = new StudentScore
            {
                CoursePoint = quizzes.Sum(s => s.TotalScore) + assignments.Sum(s => s.TotalScore),
                StudentPoint = quizzes.Sum(s => s.StudentScore) + assignments.Sum(s => s.StudentScore),
            };
            return result;
        }

        public async Task<List<StudentScormScore>> GetStudentQuizScoresScorm(Guid assignedStudentId, QuizScoreToKeepType? option)
        {
            option = option ?? QuizScoreToKeepType.Highest;
            var query = await WorkScope.GetAll<ScormTestAttempt>().Where(s => s.CourseAssignedStudentId == assignedStudentId)
                .GroupBy(s => s.Name)
               .Select(s => new  { s.Key, ListScore = s.Select(x => new { x.Score, x.MaxScore }) }).ToListAsync();
            var result = new List<StudentScormScore>();
            foreach (var item in query)
            {
                var sss = new StudentScormScore { Name = item.Key, Score = 0, MaxScore = 0 };
                if (option == QuizScoreToKeepType.Highest)
                {
                    var highest = item.ListScore.OrderByDescending(s => s.MaxScore > 0 ? s.Score / s.MaxScore : 0).FirstOrDefault();                    
                    sss.Score =  highest.Score;
                    sss.MaxScore = highest.MaxScore;
                }
                else
                {
                    sss.Score = item.ListScore.Average(s => s.Score);
                    sss.MaxScore = item.ListScore.Average(s => s.MaxScore);
                }
                result.Add(sss);
            }
            return result;

        }


        public async Task<StudentScore> GetStudentScoreScorm(Guid assignedStudentId, QuizScoreToKeepType? option)
        {
            List<StudentScormScore> quizzes = await GetStudentQuizScoresScorm(assignedStudentId, option);
            var result = new StudentScore
            {
                CoursePoint = quizzes.Sum(s => s.MaxScore),
                StudentPoint = quizzes.Sum(s => s.Score),
            };
            return result;
        }

        public async Task<CourseInstance> RePublishCourse(Guid courseInstanceId)
        {
            var expiredCourseInstance = await WorkScope.GetRepo<CourseInstance, Guid>().GetAsync(courseInstanceId);

            var newCourseInstance = new CourseInstance
            {
                CourseId = expiredCourseInstance.CourseId,
                Status = CourseSettingStatus.Active,
                AllowSkip = expiredCourseInstance.AllowSkip,
                TotalQuiz = expiredCourseInstance.TotalQuiz,
                Version = expiredCourseInstance.Version,
                AllowFinalQuizRetry = expiredCourseInstance.AllowFinalQuizRetry,
                NumberDayToStudy = expiredCourseInstance.NumberDayToStudy,
                EnableCourseGradingScheme = expiredCourseInstance.EnableCourseGradingScheme
            };
            newCourseInstance.Id = await WorkScope.InsertAndGetIdAsync(newCourseInstance);
            
            await CurrentUnitOfWork.SaveChangesAsync();

            var cg = new CourseGroup
            {
                CourseInstanceId = newCourseInstance.Id,
                Name = Const.EVERYONE_COURSE_GROUP,
                IsEveryOne = true
            };
            await WorkScope.InsertAsync<CourseGroup>(cg);

            #region Quizsettings
            var quizSettings = await WorkScope.GetAll<QuizSetting>().Where(qs => qs.CourseInstanceId == courseInstanceId).ToListAsync();
            foreach (var quizsetting in quizSettings)
            {
                var quizstId = quizsetting.Id;
                var newquizsetting = quizsetting;
                newquizsetting.Id = Guid.NewGuid();
                newquizsetting.CourseInstanceId = newCourseInstance.Id;
                newquizsetting.StartTimeUtc = newquizsetting.EndTimeUtc = null;
                newquizsetting.Id = await WorkScope.InsertAndGetIdAsync(newquizsetting);
                await CurrentUnitOfWork.SaveChangesAsync();

                var pageLinkExams =await  WorkScope.GetAll<PageLinkExam>().Where(ple => ple.LinkId == quizstId).ToListAsync();
                foreach (var pl in pageLinkExams)
                {
                    var newpl = pl;
                    newpl.Id = Guid.NewGuid();
                    newpl.LinkId = newquizsetting.Id;
                    await WorkScope.InsertAsync(newpl);
                }
            }
            #endregion

            #region AssignmentSettings
            var assignmentSettings = await WorkScope.GetAll<AssignmentSetting>().Where(ast => ast.CourseInstanceId == courseInstanceId).ToListAsync();
            foreach (var assignsetting in assignmentSettings)
            {
                var assignstId = assignsetting.Id;
                var newAssignSetting = assignsetting;
                newAssignSetting.Id = Guid.NewGuid();
                newAssignSetting.CourseInstanceId = newCourseInstance.Id;
                newAssignSetting.StartTimeUtc = newAssignSetting.EndTimeUtc = null;
                newAssignSetting.Id = await WorkScope.InsertAndGetIdAsync(newAssignSetting);
                await CurrentUnitOfWork.SaveChangesAsync();

                var pageLinkExams = await WorkScope.GetAll<PageLinkExam>().Where(ple => ple.LinkId == assignstId).ToListAsync();

                foreach (var pl in pageLinkExams)
                {
                    var newpl = pl;
                    newpl.Id = Guid.NewGuid();
                    newpl.LinkId = newAssignSetting.Id;
                    await WorkScope.InsertAsync(newpl);
                }
            }
            #endregion
            expiredCourseInstance.Status = CourseSettingStatus.Deactive;
            await WorkScope.UpdateAsync(expiredCourseInstance);
            return newCourseInstance;
        }

        public IQueryable<CourseAssignedStudent> GetCourseAssignedStudentsAcceptedAndCompleted(Guid courseInstanceId)
        {
            return WorkScope.GetAll<CourseAssignedStudent>().Where( s => s.CourseInstanceId == courseInstanceId && (s.Status == AssignedStatus.Accepted || s.Status == AssignedStatus.Completed));

        }
        public async Task CreateCourseDefaultTemplate(Guid courseId)
        {
            CourseCertificationTemplate defaultCompletedTemp = new CourseCertificationTemplate()
            {
                CourseId= courseId,
                IsActive=true,
                Background = "/assets/images/background.png",
                Content= @"<!DOCTYPE html>
                            <html>
                            <head>
                            </head>
                            <body>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><img src='data:image/jpeg;base64,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' alt='' width='243' height='83' /></p>
                            <p style='text-align: center;'><span style='font-family: arial, helvetica, sans-serif; font-size: 12pt;'><strong>NCC Online Course Academy&nbsp;</strong></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 36pt; color: #ff0000;'><span style='font-family: arial, helvetica, sans-serif;'><strong>CERTIFICATE OF ATTENDANCE&nbsp;</strong></span></span></p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'>THIS ACKNOWLEDGES THAT&nbsp;</span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 32pt; font-family: arial, helvetica, sans-serif;'>[_UserName_]</span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'>HAS SUCCESSFULLY COMPLETED </span></p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'>THE IN <span style='color: #ff0000;'>[_GradeSchemeLevel_]</span></span></p>
                            <p style='text-align: center;'> &nbsp;</p>
                            <p style='text-align: center;'><span style = 'font-size: 12pt; font-family: arial, helvetica, sans-serif;' ><strong><img src = 'data:image/png;base64,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' alt = '' width = '91' height = '45' /></strong></span></p>
                            <p style='text-align: center;'><span style = 'font-size: 16pt; color: #ff0000;' ><span style = 'font-family: arial, helvetica, sans-serif;' ><strong> Nhan Nguyen </strong></span></span></p>
                            <p style='text-align: center;'><span style = 'font-size: 12pt; font-family: arial, helvetica, sans-serif;' ><span style = 'font-family: arial, helvetica, sans-serif;' ><span style = 'font-size: 16px;' > CEO&nbsp;</span></span></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            </body>
                            </html>",
                Name = "Completed Template",
                Orientation = CertificationOrientation.Landscape,
                CertificationType = CertificationType.Completion
            };
            defaultCompletedTemp.Id = await WorkScope.InsertAndGetIdAsync<CourseCertificationTemplate>(defaultCompletedTemp);
            CourseCertificationTemplate defaultAttendanceTemp = new CourseCertificationTemplate()
            {
                CourseId = courseId,
                IsActive = true,
                Background = "/assets/images/background.png",
                Content = @"<!DOCTYPE html>
                            <html>
                            <head>
                            </head>
                            <body>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><img src='data:image/jpeg;base64,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' alt='' width='243' height='83' /></p>
                            <p style='text-align: center;'><span style='font-family: arial, helvetica, sans-serif; font-size: 12pt;'><strong>NCC Online Course Academy&nbsp;</strong></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 36pt; color: #ff0000;'><span style='font-family: arial, helvetica, sans-serif;'><strong>CERTIFICATE OF ATTENDANCE&nbsp;</strong></span></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'>THIS ACKNOWLEDGES THAT&nbsp;&nbsp;</span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 32pt; font-family: arial, helvetica, sans-serif;'>[_UserName_]</span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><span style='font-size: 16px;'>HAS SUCCESSFULLY COMPLETED </span></span></p>
                            <p style='text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><span style='font-size: 16px;'>THE IN <span style='color: #00ff00;'>[_GradeSchemeLevel_]</span></span></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'><strong><img src='data:image/png;base64,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' alt='' width='91' height='45' /></strong></span></p>
                            <p style='text-align: center;'><span style='font-size: 16pt; color: #ff0000;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Nhan Nguyen </strong></span></span></p>
                            <p style='text-align: center;'><span style='font-size: 12pt; font-family: arial, helvetica, sans-serif;'><span style='font-family: arial, helvetica, sans-serif;'><span style='font-size: 16px;'>CEO&nbsp;</span></span></span></p>
                            <p style='text-align: center;'>&nbsp;</p>
                            </body>
                            </html>",
                Name = "Attendance Template",
                Orientation = CertificationOrientation.Landscape,
                CertificationType = CertificationType.Attendance
            };
            defaultAttendanceTemp.Id = await WorkScope.InsertAndGetIdAsync<CourseCertificationTemplate>(defaultAttendanceTemp);
            await CurrentUnitOfWork.SaveChangesAsync();

        }
    }
}
