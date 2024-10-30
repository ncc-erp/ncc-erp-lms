using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.Domain.Repositories;
using RMALMS.Authorization.Roles;
using RMALMS.FeatureOptions.Dto;
using RMALMS.IoC;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Configuration;
using RMALMS.Configuration.Dto;
using Abp.Configuration;
using Abp.Authorization;

namespace RMALMS.FeatureOptions
{
    public class FeatureOptionAppService : ApplicationBaseService
    {

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public async Task<FeatureOptionDto> GetFeature()
        {
            var currentTenant = AbpSession.TenantId.Value;
            var query = from permisson in _ws.GetAll<RolePermissionSetting, long>()
                        select new PermissionRoleDto()
                        {
                            PermissonId = permisson.Id,
                            PermissionName = permisson.Name,
                            IsGranted = permisson.IsGranted,
                            RoleId = permisson.RoleId
                        };
            var qDashboard = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.DashboardDefaultView && d.TenantId == currentTenant).FirstOrDefaultAsync();
            var qStudent = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentDefaultView && d.TenantId == currentTenant).FirstOrDefaultAsync();
            var qStudentEnroll = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor && d.TenantId == currentTenant).FirstOrDefaultAsync();
            var qStudentProficiency = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentProficiencyLevelRequired && d.TenantId == currentTenant).FirstOrDefaultAsync();
            var result = new FeatureOptionDto()
            {
                Roles = await _ws.GetAll<Role, int>().Select(s => new RoleDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DisplayName = s.DisplayName
                }).ToListAsync(),
                PermissionRoles = await query.ToListAsync()
            };
            if (qDashboard != null)
            {
                result.DashboardDefaultViewName = Int32.Parse(qDashboard.Value);
            }
            if (qStudent != null)
            {
                result.StudentDefaultViewName = Int32.Parse(qStudent.Value);
            }
            if (qStudentEnroll != null)
            {
                result.StudentCourseEnrollment = Boolean.Parse(qStudentEnroll.Value);
            }
            if (qStudentProficiency != null)
            {
                result.StudentProficiency = Boolean.Parse(qStudentProficiency.Value);
            }
            return result;
        }

        [HttpPut]
        public async Task ChangeNavigator(EditPermissionRoleDto input)
        {
            var currentTenant = AbpSession.TenantId.Value;
            var currentUserId = AbpSession.UserId.Value;
            var item = await _ws.GetAll<RolePermissionSetting, long>().Where(s => s.RoleId == input.RoleId && s.Name == input.Name).FirstOrDefaultAsync();
            if (item != null)
            {
                item.IsGranted = input.IsGranted;
                await _ws.UpdateAsync<RolePermissionSetting,long>(item);
            }
            else
            {
                item = new RolePermissionSetting
                {
                    IsGranted = input.IsGranted,
                    Name = input.Name,
                    CreationTime = DateTime.UtcNow,
                    RoleId = input.RoleId,
                    TenantId = currentTenant,
                    CreatorUserId = currentUserId
                };
                item.Id = await _ws.InsertAndGetIdAsync<RolePermissionSetting, long>(item);
            }
        }

        [HttpPut]
        public async Task ChangeReports(EditPermissionRoleDto input)
        {
            var query = await _ws.GetRepo<RolePermissionSetting,long>().GetAsync(input.Id);
            query.IsGranted = input.IsGranted;
            await _ws.GetRepo<RolePermissionSetting,long>().UpdateAsync(query);
        }

        [HttpPut]
        public async Task ChangeStudentDefaultView(DefaultViewDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.StudentDefaultView, input.StudentDefaultViewName.ToString());
        }

        [HttpPut]
        public async Task ChangeDashboardDefaultView(DefaultViewDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.DashboardDefaultView, input.DashboardDefaultViewName.ToString());
        }

        [HttpPut]
        public async Task ChangeStudentEnrollment(DefaultViewDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor, input.StudentCourseEnrollment.ToString());
        }

        [HttpPut]
        public async Task ChangeStudentProficiency(DefaultViewDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.TenantId.Value, AppSettingNames.StudentProficiencyLevelRequired, input.StudentProficiency.ToString());
        }
    } 
}
