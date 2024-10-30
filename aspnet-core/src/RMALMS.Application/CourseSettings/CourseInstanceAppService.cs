using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.CourseSettings;
using RMALMS.CourseSettings.Dto;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.Helper;
//using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace RMALMS.CourseSettings
{
    [AbpAuthorize]
    public class CourseInstanceAppService : ApplicationBaseService
    {

        public  ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async Task<CourseInstanceDto> Save(CourseInstanceDto input)
        {

            CourseInstance item = null;

            if (input.Id != Guid.Empty)
            {
                item = await _ws.GetRepo<CourseInstance>().GetAsync(input.Id);
            }
            if (item == null)
            {
                item = ObjectMapper.Map<CourseInstance>(input);
                item.Id = await _ws.InsertAndGetIdAsync(item);
            }
            else
            {
                ObjectMapper.Map(input, item);
                await _ws.UpdateAsync(item);
            }
            if (input.EnableCourseGradingScheme && input.GradeSchemeId != null )
            {
                var grades = _ws.GetAll<GradeScheme>().Where(gs => gs.CourseId == input.CourseId).ToList();
                foreach (var grade in grades)
                {
                    if (grade.Id != input.GradeSchemeId)
                    {
                        if (grade.Status == GradeSchemeStatus.Active)
                        {
                            grade.Status = GradeSchemeStatus.InActive;
                            await _ws.UpdateAsync(grade);
                        }
                    }
                    else
                    {
                        grade.Status = GradeSchemeStatus.Active;
                        await _ws.UpdateAsync(grade);
                    }
                }
            }
            return input;
        }


        public async Task<CourseInstanceDto> GetById(Guid courseInstanceId)
        {
            var courseInstance = await _ws.GetRepo<CourseInstance>().GetAllIncluding(s => s.Course).FirstOrDefaultAsync(s => s.Id == courseInstanceId);
            var result = ObjectMapper.Map<CourseInstanceDto>(courseInstance);
            var activeGradeScheme = _ws.GetAll<GradeScheme>().Where(gs => gs.CourseId == courseInstance.CourseId && gs.Status == GradeSchemeStatus.Active).FirstOrDefault();
            result.GradeSchemeId = activeGradeScheme != null ? activeGradeScheme.Id : Guid.Empty;
            return result;
        }

    }
}
