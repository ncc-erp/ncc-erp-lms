using RMALMS.Entities;
using RMALMS.Scorms.Dto;
using RMALMS.TestAttempts.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Abp.UI;
using System.Xml;
using RMALMS.Web;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Xml.Linq;
using RMALMS.DomainServices;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Authorization.Users;
using Abp.Application.Services.Dto;

namespace RMALMS.Scorms
{
    public class ScormAppService : ApplicationBaseService
    {
        readonly IConfiguration _configuration;
        readonly IQuizManager _quizManager;
        readonly IUserCertificationManager _userCertificationManager;
        public ScormAppService(
            IUserCertificationManager userCertificationManager,
            IQuizManager quizManager,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _quizManager = quizManager;
            _userCertificationManager = userCertificationManager;
        }
        public async Task<StudentScormDto> CreateStudentScorm(StudentScormDto input)
        {

            var item = await _ws.GetRepo<StudentScorm>().GetAllIncluding(s => s.CourseAssignedStudent)
                .Where(s => s.CourseAssignedStudentId == input.CourseAssignedStudentId && s.Key == input.Key)
                .LastOrDefaultAsync();

            if (item != null && item.CourseAssignedStudent != null && item.CourseAssignedStudent.StudentId != AbpSession.UserId)
            {
                throw new UserFriendlyException(string.Format("User id {0} is trying to fake user id {1}", AbpSession.UserId, item.CourseAssignedStudent.StudentId));
            }
            if (item != null && (item.CourseAssignedStudent == null || item.CourseAssignedStudent.Status != AssignedStatus.Accepted))
            {
                throw new UserFriendlyException(string.Format("User id {0} is not accepted to this course or completed this course", AbpSession.UserId));
            }
            if (item != null)
            {
                if (input.Value != item.Value)
                {
                    item.Value = input.Value;
                    await _ws.UpdateAsync(item);
                }
            }
            else
            {
                item = new StudentScorm
                {
                    CourseAssignedStudentId = input.CourseAssignedStudentId,
                    Key = input.Key,
                    Value = input.Value
                };
                item.Id = await _ws.InsertAndGetIdAsync(item);
            }

            return ObjectMapper.Map<StudentScormDto>(item);

        }

        public async Task<StudentScormDto> UpdateStudentScorm(StudentScormDto input)
        {
            var item = await _ws.GetRepo<StudentScorm>().GetAsync(input.Id);
            ObjectMapper.Map<StudentScormDto, StudentScorm>(input, item);
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<StudentScormDto>(item);
        }

        public async Task<List<StudentScormDto>> GetStudentScorms(Guid courseAssignedStudentId)
        {
            return await _ws.GetAll<StudentScorm>()
                .Where(s => s.CourseAssignedStudentId == courseAssignedStudentId)
                .ProjectTo<StudentScormDto>()
                .ToListAsync();
        }

        public async Task<List<CourseScormPageDto>> GetCourseNavigation(Guid courseAssignedStudentId)
        {
            //var courseAssignedStudent = await _ws.GetRepo<CourseAssignedStudent>().GetAllIncluding(a => a.CourseInstance, a => a.CourseInstance.Course).Where(a => a.Id == id).FirstOrDefaultAsync();
            //var filePath = courseAssignedStudent.CourseInstance.Course.SoursePath.Replace("shared/launchpage.html", "imsmanifest.xml");
            var filePath = await _ws.GetRepo<CourseAssignedStudent>().GetAllIncluding(a => a.CourseInstance, s => s.CourseInstance.Course)
                .Where(a => a.Id == courseAssignedStudentId).Select(s => s.CourseInstance.Course.SoursePath).FirstOrDefaultAsync();

            filePath = filePath.Replace("shared/launchpage.html", "imsmanifest.xml");
            var wwwFolder = _configuration["WWWFolder"];
            var fullPath = $"{WebContentDirectoryFinder.RootFolder}\\{wwwFolder}\\{filePath}";
            if (File.Exists(fullPath))
            {
                try
                {
                    //get Student progress scorm
                    var readPageIds = await _ws.GetAll<StudentProgressScorm>().Where(s => s.CourseAssignedStudentId == courseAssignedStudentId)
                        .Select(s => s.PageId).ToListAsync();

                    XDocument doc = XDocument.Load(fullPath);
                    var manifest = doc.Elements().Where(e => e.Name.LocalName == "manifest");
                    var organizations = manifest.Elements().Where(e => e.Name.LocalName == "organizations");
                    var resourceslist = manifest.Elements().Where(e => e.Name.LocalName == "resources");
                    var root = organizations.Elements().Where(x => x.Name.LocalName == "organization");
                    var resources = resourceslist.Elements().Where(x => x.Name.LocalName == "resource");
                    List<CourseScormPageDto> units = LoadTreeNode(root, 0, resources, readPageIds);
                    return units;
                }
                catch (Exception e)
                {
                    Logger.Error(String.Format("Scorm course at {0} is invalid data. error >> {1}", fullPath, e.Message));
                    throw new UserFriendlyException(String.Format("Scorm course at {0} is not exist", fullPath));
                }
            }
            else
                Logger.Error(String.Format("Scorm course at {0} is not exist", fullPath));
            throw new UserFriendlyException();
        }


        private List<CourseScormPageDto> LoadTreeNode(IEnumerable<XElement> units, int level, IEnumerable<XElement> resources, List<string> readPageIds)

        {
            level++;

            var element = units.Select(x => new CourseScormPageDto()
            {
                Id = x.Attribute("identifier").Value,

                Href = x.Attribute("identifierref") != null ?
                        GetHrefFromSource(x.Attribute("identifierref").Value, resources) + (x.Attribute("parameters") != null ? x.Attribute("parameters").Value : "")
                        : null,

                Name = x.Elements().Where(e => e.Name.LocalName == "title") != null ?
                        x.Elements().Where(e => e.Name.LocalName == "title").FirstOrDefault().Value
                        : "No Name",

                Level = level,
                IsDone = readPageIds.Any(s => s == x.Attribute("identifier").Value),
                Children = LoadTreeNode(x.Elements().Where(e => e.Name.LocalName == "item"), level, resources, readPageIds)

            }).ToList();
            return element.Count > 0 ? element : null;
        }


        private string GetHrefFromSource(string identifier, IEnumerable<XElement> resources)
        {
            var href = string.Empty;
            var resouce = resources.Where(r => r.Attribute("identifier") != null && r.Attribute("identifier").Value == identifier).FirstOrDefault();
            if (resouce != null && resouce.Attribute("href") != null)
                href = resouce.Attribute("href").Value;
            return href;

        }


        #region student progress for secorm
        public async Task<StudentProgressScormDto> CreateStudentProgressScorm(StudentProgressScormDto input)
        {
            var isExist = await _ws.GetAll<StudentProgressScorm>().AnyAsync(sp => sp.CourseAssignedStudentId == input.CourseAssignedStudentId && sp.PageId == input.PageId);
            if (!isExist)
            {
                var item = ObjectMapper.Map<StudentProgressScorm>(input);
                //item.Progress = StudentProgressStatus.Completed;
                input.Id = await _ws.InsertAndGetIdAsync(item);
                return input;
            }
            else
            {
                return null;
            }

        }

        public async Task<StudentProgressScormDto> UpdateStudentProgressScorm(StudentProgressScormDto input)
        {
            var item = await _ws.GetRepo<StudentProgressScorm>().GetAsync(input.Id);
            ObjectMapper.Map<StudentProgressScormDto, StudentProgressScorm>(input, item);
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<StudentProgressScormDto>(item);

        }

        #endregion

        public async Task<ScormTestAttemptDto> CreateScormTestAttempt(ScormTestAttemptDto input)
        {
            var item = ObjectMapper.Map<ScormTestAttempt>(input);
            if (item.CourseAssignedStudentId == null)
                throw new UserFriendlyException(String.Format("User id {0} has not accepted to this course", AbpSession.UserId.Value));

            item.Status = TestAttemptStatus.Marking;
            if (string.IsNullOrEmpty(item.Name))
                item.Name = input.IsFinal ? "Final Quiz" : "Quiz";
            input.Id = await _ws.InsertAndGetIdAsync(item);
            if (input.IsFinal)
            {
                await _quizManager.CompletedCourse(item.CourseAssignedStudentId);
                //await _userCertificationManager.CreateUpdateUserCertificationScorm(item.CourseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }
            return input;
        }

        public async Task CompletedCourseScorm12(EntityDto<Guid> input)
        {
            var cas = await _ws.GetRepo<CourseAssignedStudent>().GetAsync(input.Id);
            if (cas.Status != AssignedStatus.Accepted)
            {
                throw new UserFriendlyException(String.Format("User id {0} has not accepted to this course", AbpSession.UserId.Value));
            }
            await _quizManager.CompletedCourse(input.Id);
            
        }


        public async Task<ScormStatisticDto> GetScormStatistics(Guid courseInstanceId)
        {

            var result = new ScormStatisticDto();
            var scoreCalculateType = 0; // 0 = Highest, 1 = Avarage; Default is 0
            var courseAssignStudent = from cas in _ws.GetAll<CourseAssignedStudent>().Where(c => c.CourseInstanceId == courseInstanceId) select cas;
            var scormAttempts = from at in _ws.GetRepo<ScormTestAttempt>().GetAllIncluding(a => a.CourseAssignedStudent)
                                join cas in _ws.GetAll<CourseAssignedStudent>().Where(c => c.CourseInstanceId == courseInstanceId)
                                on at.CourseAssignedStudentId equals cas.Id
                                select at;
            var queryQuiz = from at in scormAttempts
                            group at by at.Name into gat
                            select new StudentQuizDto { Name = gat.Select(s => s.Name).FirstOrDefault(), Score = gat.Select(s => s.MaxScore).FirstOrDefault() };
            result.Quizzes = await queryQuiz.ToListAsync();
            //result.StudentsResult =  (from at in scormAttempts
            //                               group at by new { at.Name, at.CourseAssignedStudent } into studentScormAttempts
            //                               join u in _ws.GetRepo<User, long>().GetAll() on
            //                               studentScormAttempts.FirstOrDefault().CourseAssignedStudent.StudentId equals u.Id
            //                               select new { Name = u.Name, StudentId = u.Id, Score = studentScormAttempts.Average(a => a.Score), MaxScore = studentScormAttempts.FirstOrDefault().MaxScore } into studentattempts
            //                               group studentattempts by studentattempts.StudentId into lastresult
            //                               select new StudentScormResultDto {
            //                                   Name = lastresult.FirstOrDefault().Name,
            //                                   StudentId = lastresult.Key,
            //                                   QuizzesScore = lastresult.Select(l => l.Score).ToList(),
            //                                   Progress = lastresult.Sum(l=>l.Score)/lastresult.Sum(l=>l.MaxScore) }
            //                         ).ToList();
            var queryStudentAttempts = from at in scormAttempts
                                       group at by at.CourseAssignedStudent into studentScormAttempts
                                       join u in _ws.GetRepo<User, long>().GetAll() on
                                       studentScormAttempts.FirstOrDefault().CourseAssignedStudent.StudentId equals u.Id
                                       select new StudentStatistic
                                       {
                                           Name = u.Name,
                                           StudentId = u.Id
                                       };
            result.StudentsResult = queryStudentAttempts.ToList();
            var attemptSource = scormAttempts.ToList();
            var TotalScore = result.Quizzes.Sum(q => q.Score);
            foreach (var item in result.StudentsResult)
            {
                item.QuizzesScore = CalculateScore(item.StudentId, attemptSource, result.Quizzes, scoreCalculateType);
                item.Progress = (item.QuizzesScore.Sum() / TotalScore) * 100;
            }
            return result;
        }
        private List<float> CalculateScore(long studentId, List<ScormTestAttempt> attemptSource, List<StudentQuizDto> quizSource, int type)
        {
            var score = new List<float>();
            foreach (var item in quizSource)
            {
                var scoreitem = attemptSource.Where(a => a.Name == item.Name && a.CourseAssignedStudent.StudentId == studentId).GroupBy(a => a.CourseAssignedStudentId).Select(a => (type == 0) ? a.Max(i => i.Score) : a.Average(i => i.Score));
                score.Add(scoreitem != null ? scoreitem.FirstOrDefault() : 0);
            }
            return score;
        }
        public async Task<StudentScormGradeDto> GetStudentQuizProgress(Guid courseInstanceId)
        {
            var result = new StudentScormGradeDto();
            var userId = AbpSession.UserId.Value;
            var scoreCalculateType = 0; // 0 = Highest, 1 = Avarage; Default is 0
            var courseAssignStudent = from cas in _ws.GetAll<CourseAssignedStudent>().Where(c => c.CourseInstanceId == courseInstanceId) select cas;
            var scormAttempts = from at in _ws.GetRepo<ScormTestAttempt>().GetAllIncluding(a => a.CourseAssignedStudent)
                                join cas in _ws.GetAll<CourseAssignedStudent>().Where(c => c.CourseInstanceId == courseInstanceId)
                                on at.CourseAssignedStudentId equals cas.Id
                                select at;
            var queryQuiz = from at in scormAttempts
                            group at by at.Name into gat
                            select new StudentQuizDto { Name = gat.Select(s => s.Name).FirstOrDefault(), Score = gat.Select(s => s.MaxScore).FirstOrDefault() };
            result.Quizzes = await queryQuiz.ToListAsync();
            var queryStudentAttempts = from at in scormAttempts
                                       group at by at.CourseAssignedStudent into studentScormAttempts
                                       join u in _ws.GetRepo<User, long>().GetAll() on
                                       studentScormAttempts.FirstOrDefault().CourseAssignedStudent.StudentId equals u.Id
                                       select new StudentStatistic
                                       {
                                           Name = u.Name,
                                           StudentId = u.Id
                                       };
            var StudentsProgress = queryStudentAttempts.ToList();
            var attemptSource = scormAttempts.ToList();
            var TotalScore = result.Quizzes.Sum(q => q.Score);
            result.StudentsProgress = new List<StudentStatistic>();
            foreach (var item in StudentsProgress)
            {
                var quizzesScore = CalculateScore(item.StudentId, attemptSource, result.Quizzes, scoreCalculateType);
                item.Progress = (quizzesScore.Sum() / TotalScore) * 100;
                if (item.StudentId != userId)
                    result.StudentsProgress.Add(item);
                else
                {
                    item.QuizzesScore = quizzesScore;
                    result.StudentStatistic = item;
                }
            }
            return result;
        }

    }
}
