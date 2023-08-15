using Microsoft.EntityFrameworkCore;
using RMALMS.DomainServices.Entity;
using RMALMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMALMS.DomainServices
{
    public class TestAttemptManager : BaseDomainService, ITestAttemptManager
    {
        public async Task<List<TestingTestAttempt>> GetAllTestingTestAttempt()
        {
            return await (from ta in WorkScope.GetAll<TestAttempt>()
                          join qzs in WorkScope.GetAll<QuizSetting>() on ta.QuizSettingId equals qzs.Id
                          join qz in WorkScope.GetAll<Quiz>() on qzs.QuizId equals qz.Id
                          join ple in WorkScope.GetAll<PageLinkExam>() on qzs.Id equals ple.LinkId
                          join p in WorkScope.GetAll<Page>() on ple.PageId equals p.Id
                          where
                              ta.Status == TestAttemptStatus.Testing &&
                              qz.TimeLimit.HasValue &&
                              ta.CreatorUserId.HasValue &&
                              (p.Type == PageType.Quiz || p.Type == PageType.QuizFinal)
                          select new TestingTestAttempt
                          {
                              TestAttemptId = ta.Id,
                              PageType = p.Type,
                              StudentId = ta.CreatorUserId.Value,
                              CreationTime = ta.CreationTime,
                              TimeLimit = qz.TimeLimit
                          }).ToListAsync();
        }
    }
}
