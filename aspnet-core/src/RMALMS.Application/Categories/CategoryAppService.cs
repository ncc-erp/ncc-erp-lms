using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Categories.Dto;
using RMALMS.Entities;
using RMALMS.Extension;
//using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Categories
{
    //[AbpAuthorize(Authorization.PermissionNames.Pages_Categories)]
    [AbpAuthorize]
    public class CategoryAppService : AsyncCrudAppService<Category, CategoryDto, Guid, PagedResultRequestDto, CreateCategoryDto, CategoryDto>, ICategoryAppService
    {
        private readonly IWorkScope _ws;
        public CategoryAppService(IRepository<Category, Guid> repository, IWorkScope workScope) : base(repository)
        {
            _ws = workScope;
        }


        [AbpAuthorize(Authorization.PermissionNames.Pages_Categories)]
        [HttpPost]
        public async Task<GridResult<CategoryDto>> GetAllPagging(GridParam input)
        {
            var query = Repository.GetAll().ProjectTo<CategoryDto>();
            return await query.GetGridResult(query, input);
        }

        public async Task<ListResultDto<CategoryDto>> GetAllNotPagging()
        {
            var query = Repository.GetAll().ProjectTo<CategoryDto>();
            var categories = await query.ToListAsync();
            return new ListResultDto<CategoryDto>(categories);
        }

        public  ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }
        [AbpAuthorize(Authorization.PermissionNames.Pages_Categories)]
        public async override Task<CategoryDto> Create(CreateCategoryDto input)
        {
            CheckCreatePermission();
            var category = ObjectMapper.Map<Category>(input);
            category.Id = await _ws.InsertAndGetIdAsync(category);
            return MapToEntityDto(category);            
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Categories)]
        public async override Task<CategoryDto> Update(CategoryDto input)
        {
            CheckUpdatePermission();
            var category = Repository.Get(input.Id);
            MapToEntity(input, category);
            await Repository.UpdateAsync(category);
            return input;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Categories)]
        public override Task Delete(EntityDto<Guid> input)
        {
            return base.Delete(input);
        }
    }
}
