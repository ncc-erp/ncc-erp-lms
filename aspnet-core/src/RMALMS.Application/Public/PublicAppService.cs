using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.DomainServices;
using RMALMS.DomainServices.Entity;
using RMALMS.Entities;
using RMALMS.IoC;
using RMALMS.Ncc;
using RMALMS.TestAttempts.Dto;
using RMALMS.Uitls;
using RMALMS.Users.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace RMALMS.Public
{
    public class PublicAppService : ApplicationBaseService
    {
        private readonly IQuizManager _quizManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IWorkScope _ws;


        public PublicAppService(
            IQuizManager quizManager,
            UserManager userManager,
            RoleManager roleManager,
            IWorkScope workScope
        ) : base()
        {
            _quizManager = quizManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _ws = workScope;
        }
        public async Task<TestAttemptDto> ProcessScore(long studentId, Guid testAttemptId)
        {
            var testAttempt = _ws.GetAll<TestAttempt>().Where(s => s.Id == testAttemptId).FirstOrDefault();
            if (testAttempt.CreationTime.Date >= DateTimeUtils.GetNow().Date)
            {
                throw new UserFriendlyException($"testAttempt.CreationTime: {testAttempt.CreationTime}, DateTimeUtils.GetNow(): {DateTimeUtils.GetNow()}");
            }

            var item = await _quizManager.ProcessScore(studentId, testAttemptId, Entities.PageType.Quiz);
            var itemDto = ObjectMapper.Map<TestAttemptDto>(item);
            return itemDto;
        }
        [NccAuth]
        public async Task<object> CreateUserHRM(CreateUserDto input)

        {

            var user = ObjectMapper.Map<User>(input);
            user.CreatorUserId = AbpSession.UserId;
            user.TenantId = input.TenantId.HasValue ? input.TenantId : AbpSession.TenantId;
            user.IsEmailConfirmed = true;
            Entities.UserStatus userLevel = null;
            if (input.TenantId > 0)
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    userLevel = await _ws.GetRepo<Entities.UserStatus, Guid>().GetAll().OrderBy(m => m.Level).FirstOrDefaultAsync();
                    user.Roles = new Collection<UserRole>();
                    var role = await _roleManager.GetRoleByNameAsync("STUDENT");
                    user.Roles.Add(new UserRole(input.TenantId, user.Id, role.Id));
                }
            }
            else
            {
                userLevel = await _ws.GetRepo<Entities.UserStatus, Guid>().GetAll().OrderBy(m => m.Level).FirstOrDefaultAsync();
                user.Roles = new Collection<UserRole>();
                var role = await _roleManager.GetRoleByNameAsync("STUDENT");
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            if (userLevel != null)
            {
                user.StatusId = userLevel.Id;
            }

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            CurrentUnitOfWork.SaveChanges();

            var NCCEmployeeGroupId = RMALMSConsts.NCCEmployeeGroupId;


            var userGroup = new UserGroup
            {
                UserId = user.Id,
                GroupId = new Guid(NCCEmployeeGroupId)
            };

            await _ws.InsertAsync(userGroup);

            var studentId = user.Id;
            var CourseInstanceId = new Guid(RMALMSConsts.CourseInstanceId);

            var isExist = await _ws.GetAll<CourseAssignedStudent>().AnyAsync(cas => cas.StudentId == studentId && cas.CourseInstanceId == CourseInstanceId);
            if (!isExist)
            {
                //not exist mean that student was invited by group -> insert a row into CourseAssignedStudent
                var item = new CourseAssignedStudent
                {
                    StudentId = studentId,
                    CourseInstanceId = CourseInstanceId,
                    AssignedSource = AssignedSource.Direct,
                    Status = AssignedStatus.Accepted
                };
                item.Id = await _ws.InsertAndGetIdAsync(item);
            }
            else
            {
                //invited by CourseAssignedStudent -> update
                var item = await _ws.GetAll<CourseAssignedStudent>().Where(cas => cas.StudentId == studentId && cas.CourseInstanceId == CourseInstanceId).FirstOrDefaultAsync();
                item.Status = AssignedStatus.Accepted;
                await _ws.GetRepo<CourseAssignedStudent>().UpdateAsync(item);
            }

            var data = new UserDto
            {
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                IsActive = user.IsActive,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleNames = new string[] { "STUDENT" }
            };
            return data;

        }



        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<Object> ProcessScoreAll()
        {
            var testAttempts = _ws.GetAll<TestAttempt>()
                .Where(s => s.Status == TestAttemptStatus.Testing)
                .Where(s => s.CreatorUserId.HasValue)
                .Where(s => s.QuizSetting.Quiz.Type == QuizType.Quiz)
                .Where(s => !s.Score.HasValue || s.Score.Value == 0)
                .Where(s => s.CreationTime.Date < DateTimeUtils.GetNow().Date)
                .Select(s => new
                {
                    s.Id,
                    Score = s.Score ?? 0,
                    studentId = s.CreatorUserId.Value
                }).ToList();



            var result = new List<TestAttemptDto>();
            foreach (var ta in testAttempts)
            {
                var item = await _quizManager.ProcessScore(ta.studentId, ta.Id, PageType.Quiz);
                var itemDto = ObjectMapper.Map<TestAttemptDto>(item);
                result.Add(itemDto);
            }

            var notChanges = (from r in result
                              join o in testAttempts on r.Id equals o.Id
                              where r.Score == o.Score
                              select new
                              {
                                  o.studentId,
                                  o.Score
                              }).ToList();

            if (notChanges == null || notChanges.Count == 0)
            {
                return new
                {
                    testAttemptsBefor = testAttempts,
                    testAttemptsAfter = result,
                };
            }


            var Ids = notChanges.Select(s => s.studentId);
            var userNamesNoChange = _ws.GetAll<User, long>()
                .Where(s => Ids.Contains(s.Id))
                .Select(s => new { s.Id, s.UserName })
                .ToList();

            var resultsNoChange = (from n in notChanges
                                   join u in userNamesNoChange on n.studentId equals u.Id
                                   select new { n.Score, u.UserName }).ToList();

            return new
            {
                testAttemptsBefor = testAttempts,
                testAttemptsAfter = result,
                notChanges = resultsNoChange
            };
        }
        [AbpAllowAnonymous]
        [HttpGet]
        [NccAuth]
        public GetResultConnectDto CheckConnect()
        {
            return new GetResultConnectDto
            {
                IsConnected = true,
                Message = "Connected"
            };
        }
    }
}
