using Abp.Application.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services.Dto;
using RMALMS.Roles.Dto;
using System.Threading.Tasks;
using Abp.Authorization;
using RMALMS.Entities;
using System;
using RMALMS.Authorization.Roles;
using Abp.Domain.Repositories;
using RMALMS.Authorization.Users;
using RMALMS.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Abp.IdentityFramework;
using System.Collections.Generic;
using RMALMS.IoC;
using AutoMapper.QueryableExtensions;
using RMALMS.GradeSchemes.Dto;
using RMALMS.Paging;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Extension;

namespace RMALMS.GradeSchemes
{

    [AbpAuthorize(Authorization.PermissionNames.Pages_Courses)]
    public class GradeSchemeAppService : AsyncCrudAppService<GradeScheme, GradeSchemeDto, Guid, PagedResultRequestDto, CreateGradeSchemeDto, GradeSchemeDto>, IGradeSchemeAppService
    {

        private readonly IWorkScope _workScope;
        public GradeSchemeAppService(
            IRepository<GradeScheme, Guid> repository,
            IWorkScope workScope
         )
            : base(repository)
        {
            _workScope = workScope;
        }
        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async Task<List<GradeSchemeDto>> GetGradesByCourseId(Guid courseId)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == courseId).OrderBy(g => g.CreationTime).ProjectTo<GradeSchemeDto>();
            return await query.ToListAsync();
        }

        public override async Task<GradeSchemeDto> Create(CreateGradeSchemeDto input)
        {
            CheckCreatePermission();
            var grade = ObjectMapper.Map<GradeScheme>(input);
            grade.Status = GradeSchemeStatus.InActive;
            grade.Id = await _workScope.InsertAndGetIdAsync(grade);
            foreach (var item in input.ElementList)
            {
                var element = ObjectMapper.Map<GradeSchemeElement>(item);
                element.GradeSchemeId = grade.Id;
                await _workScope.InsertAsync(element);
            }
            return MapToEntityDto(grade);
        }
        public override async Task<GradeSchemeDto> Update(GradeSchemeDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            var oldElements = _workScope.GetAll<GradeSchemeElement, Guid>().Where(el => el.GradeSchemeId == input.Id).ToList();
            var currentElements = new List<Guid>();
            foreach (var e in input.ElementList)
            {
                var element = new GradeSchemeElement();
                if (e.Id != Guid.Empty)
                {
                    currentElements.Add(e.Id);
                    element = _workScope.GetAll<GradeSchemeElement, Guid>().Where(el => el.Id == e.Id).FirstOrDefault();
                    ObjectMapper.Map(e, element);
                    await _workScope.UpdateAsync(element);
                }
                else
                {
                    element = ObjectMapper.Map<GradeSchemeElement>(e);
                    element.GradeSchemeId = input.Id;
                    await _workScope.InsertAsync(element);
                }
            }
            await _workScope.GetRepo<GradeSchemeElement>().DeleteAsync(e => (e.GradeSchemeId == input.Id && !currentElements.Contains(e.Id)));
            return input;

        }

        [HttpPost]
        public async Task<GridResult<GradeSchemeDto>> getGradesByCourseIdPagging(GridGradeParam input)
        {
            var query = Repository.GetAll().Where(g => g.CourseId == input.courseId ).OrderByDescending(g=> g.CreationTime ).ProjectTo<GradeSchemeDto>();

            return await query.GetGridResult(query, input.input);
        }

    }
}
