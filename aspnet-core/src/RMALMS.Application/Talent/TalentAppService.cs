using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Entities;
using RMALMS.Ncc;
using RMALMS.Talent.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static RMALMS.Authorization.Roles.StaticRoleNames;

namespace RMALMS.Talent
{
    public class TalentAppService : RMALMSAppServiceBase
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IRepository<User, long> _userRepo;
        public TalentAppService(
            RoleManager roleManager,
            UserManager userManager,
            IRepository<User, long> userRepo
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepo = userRepo;
        }

        [HttpPost]
        [NccAuth]
        public async Task<StudentTalentDto> CreateAccountStudent(StudentTalentDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var count = await _userRepo.GetAll().Where(q => q.UserName.Contains(input.UserName) || q.EmailAddress.Contains(input.EmailAddress)).CountAsync();
                if (count > 0)
                {
                    input.UserName += "_" + count ;
                    input.EmailAddress = "a_" + count + "_" + input.EmailAddress;
                }

                var user = ObjectMapper.Map<User>(input);
                var roleStudent = await _roleManager.GetRoleByNameAsync(Tenants.Student);
                user.Roles = new Collection<UserRole>();
                user.Roles.Add(new UserRole
                {
                    RoleId = roleStudent.Id,
                    UserId = user.Id,
                    TenantId = AbpSession.TenantId
                });
                CheckErrors(await _userManager.CreateAsync(user, input.Password));
                CurrentUnitOfWork.SaveChanges();

                await AssignCourseForStudent(input.CourseInstanceId, user.Id);

                return input;
            }
        }
        private async Task AssignCourseForStudent(Guid courseInstanceId, long studentId)
        {
            var isExistingCourse = _ws.GetRepo<CourseInstance>()
                .GetAll()
                .Any(ci => ci.Id == courseInstanceId);
            if (!isExistingCourse)
                throw new Exception(string.Format("The course instance id {0} is not exist", courseInstanceId));

            var CASByCourses = _ws.GetAll<CourseAssignedStudent>()
                .Where(cas => cas.CourseInstanceId == courseInstanceId);

            var courseAssignStudent = new CourseAssignedStudent
            {
                CourseInstanceId = courseInstanceId,
                StudentId = studentId,
                AssignedSource = AssignedSource.Direct,
                Status = AssignedStatus.Invited
            };
            await _ws.InsertAsync(courseAssignStudent);
        }
        [HttpGet]
        [NccAllowAnonymous]
        public async Task<List<TalentCourseDto>> GetListCourse()
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var qcourseDashboard =
                   (from ci in _ws.GetAll<CourseInstance>()
                    join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                    where ci.Status == CourseSettingStatus.Active
                    select new TalentCourseDto
                    {
                        Id = ci.Id,
                        StartDate = ci.StartTime,
                        EndDate = ci.EndTime,
                        Name = c.Name,
                        Description = c.Description,
                        RelationInfo = c.RelatedInformation,
                        ImageCover = c.ImageCover,
                    }).OrderBy(s => s.StartDate);
                return await qcourseDashboard.ToListAsync();
            }
        }
    }
}
