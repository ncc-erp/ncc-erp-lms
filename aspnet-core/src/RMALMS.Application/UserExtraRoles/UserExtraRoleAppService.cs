using RMALMS.Authorization.Roles;
using RMALMS.DomainServices;
using RMALMS.UserExtraRoles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using RMALMS.Entities;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace RMALMS.UserExtraRoles
{
    public class UserExtraRoleAppService : ApplicationBaseService
    {
        private readonly IUserServices _userService;

        public UserExtraRoleAppService(IUserServices userService)
        {
            _userService = userService;
        }

        public async Task<List<SelectCourseAdminDto>> GetCourseAdminsByCourseId(Guid courseId)
        {

            var courseOwnerId = await _ws.GetAll<Course>().Where(c => c.Id == courseId).Select(c => c.CreatorUserId).FirstOrDefaultAsync();
            var roleCourseAdminId = await _ws.GetAll<Role, int>().Where(r => r.Name == StaticRoleNames.Tenants.CourseAdmin).Select(r => r.Id).FirstOrDefaultAsync();
            var quser =
                from u in _userService.GetUserByRole(StaticRoleNames.Tenants.CourseAdmin).Where(user => user.Id != courseOwnerId)
                join uer in _ws.GetAll<UserExtraRole>()
                .Where(s => s.EntityId == courseId && s.RoleId == roleCourseAdminId && s.EntityType == nameof(Course)) 
                on u.Id equals uer.UserId into groupUERs
                select new SelectCourseAdminDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    IsSelected = groupUERs.Any()
                };
            return await quser.ToListAsync();

        }

        public async Task AddCourseAdminsToCourse(CourseAdminsToCourseDto input)
        {
            var isExistingCourse = await _ws.GetAll<Course>().AnyAsync(c => c.Id == input.CourseId);
            if (!isExistingCourse) throw new UserFriendlyException(String.Format("The course id {0} is not exist", input.CourseId));
            var roleCourseAdminId = await _ws.GetAll<Role, int>().Where(r => r.Name == StaticRoleNames.Tenants.CourseAdmin).Select(r => r.Id).FirstOrDefaultAsync();

            var alreadyList = await _ws.GetAll<UserExtraRole>()
                .Where(uer => uer.EntityId == input.CourseId && uer.RoleId == roleCourseAdminId && uer.EntityType == nameof(Course))
                .Select(s => s.UserId).ToListAsync();

            //insert
            var insertList = input.CourseAdminIds.Except(alreadyList);
            foreach (var userId in insertList)
            {
                var uer = new UserExtraRole
                {
                    EntityId = input.CourseId,
                    EntityType = nameof(Course),
                    RoleId = roleCourseAdminId,
                    UserId = userId
                };
                await _ws.InsertAsync<UserExtraRole>(uer);
            }

            //delete
            var deleteList = alreadyList.Except(input.CourseAdminIds);
            await _ws.GetRepo<UserExtraRole>().DeleteAsync(uer => deleteList.Contains(uer.UserId) && uer.EntityId == input.CourseId && uer.RoleId == roleCourseAdminId && uer.EntityType == nameof(Course));

        }

    }
}
