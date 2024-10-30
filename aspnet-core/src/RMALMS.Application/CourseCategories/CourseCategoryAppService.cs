using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RMALMS.CourseCategories.Dto;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using RMALMS.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.CourseCategories
{
    [AbpAuthorize]
    public class CourseCategoryAppService : AsyncCrudAppService<CourseTag, CourseTagDto, Guid, PagedResultRequestDto, CreateCourseTagDto, CourseTagDto>
    {
        private readonly IWorkScope _ws;
        public CourseCategoryAppService(IRepository<CourseTag, Guid> repository, IWorkScope workScope) : base(repository)
        {
            _ws = workScope;
        }

        public async Task<ListResultDto<TagsByCourseDto>> GetTagsByCourseInstanceId(Guid courseInstanceId)
        {
            var query = from ct in Repository.GetAllIncluding(ct => ct.Category)
                        join ci in _ws.GetAll<CourseInstance>() on ct.CourseId equals ci.CourseId
                        where ci.Id == courseInstanceId
                        select new TagsByCourseDto() { Id = ct.CategoryId, Name = ct.Category.Name };
            
            var result = await query.ToListAsync();
            return new ListResultDto<TagsByCourseDto>(result);
        }

        public async Task AddTagsToCourse(TagsToCourseDto input)
        {
            //Check course existing
            var isExistCourse = await _ws.GetRepo<Course>().GetAll().AnyAsync(u => u.Id == input.CourseId);

            if (!isExistCourse) throw new UserFriendlyException(string.Format("The course id {0} is not exist", input.CourseId));

            var alreadyList = await _ws.GetRepo<CourseTag>().GetAll().Where(ug => ug.CourseId == input.CourseId).Select(ug => ug.CategoryId).ToListAsync();

            //insert
            var insertList = input.Tags.Except(alreadyList);
            foreach (var categoryId in insertList)
            {
                var item = new CourseTag
                {
                    CourseId = input.CourseId,
                    CategoryId = categoryId
                };
                await _ws.InsertAsync<CourseTag>(item);
            }

            //delete
            var deleteList = alreadyList.Except(input.Tags);
            await _ws.GetRepo<CourseTag>().DeleteAsync(ug => deleteList.Contains(ug.CategoryId) && ug.CourseId == input.CourseId);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public override Task<CourseTagDto> Update(CourseTagDto input)
        {
            throw new NotImplementedException();
        }
    }
}
