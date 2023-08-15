using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Entities;
using RMALMS.Extension;
//using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Modules.Dto;
using RMALMS.Paging;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Abp.UI;
using RMALMS.Uitls;

namespace RMALMS.Modules
{
    [AbpAuthorize]
    public class ModuleAppService : AsyncCrudAppService<Module, ModuleDto, Guid, PagedResultRequestDto, CreateModuleDto, ModuleDto>, IModuleAppService
    {
        private readonly IWorkScope _ws;
        public ModuleAppService(IRepository<Module, Guid> repository, IWorkScope workScope) : base(repository)
        {
            _ws = workScope;
        }


        [HttpPost]
        public async Task<GridResult<ModuleDto>> GetAllPagging(GridParam input)
        {
            var query = Repository.GetAll().ProjectTo<ModuleDto>();
            return await query.GetGridResult(query, input);
        }

        public async Task<ListResultDto<ModuleDto>> GetAllNotPagging()
        {
            var query = Repository.GetAll().ProjectTo<ModuleDto>();
            var items = await query.ToListAsync();
            return new ListResultDto<ModuleDto>(items);
        }

        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async override Task<ModuleDto> Create(CreateModuleDto input)
        {
            CheckCreatePermission();
            var item = ObjectMapper.Map<Module>(input);
            item.Identifier = DateTime.Now.ToFileTimeUtc().ToString();
            item.SequenceOrder = _ws.GetAll<Module, Guid>().Count(s => s.CourseId == input.CourseId);
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return MapToEntityDto(item);
        }

        public async override Task<ModuleDto> Update(ModuleDto input)
        {
            CheckUpdatePermission();
            var item = Repository.Get(input.Id);
            MapToEntity(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }

        public async Task<ListResultDto<ModuleDto>> GetAllByCourseId(Guid courseId)
        {
            var query = Repository.GetAll().Where(m => m.CourseId == courseId).OrderBy(m => m.SequenceOrder).ProjectTo<ModuleDto>();
            var items = await query.ToListAsync();
            return new ListResultDto<ModuleDto>(items);
        }

        public async override Task Delete(EntityDto<Guid> input)
        {
            //delete pages of this module
            await _ws.GetRepo<Page>().DeleteAsync(p => p.ModuleId == input.Id);

            await _ws.GetRepo<Module>().DeleteAsync(input.Id);
        }

        public async Task<List<CModuleDto>> GetModulesPagesByCourseId(Guid courseId)
        {
            var query =
                from m in _ws.GetAll<Module, Guid>().Where(m => m.CourseId == courseId).OrderBy(m => m.SequenceOrder)
                join p in _ws.GetAll<Page, Guid>() on m.Id equals p.ModuleId into pages
                select new CModuleDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    SequenceOrder = m.SequenceOrder,
                    Pages = pages.OrderBy(p => p.SequenceOrder).Select(s => new CPageDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        SequenceOrder = s.SequenceOrder
                    })
                };
            //return new ListResultDto<CourseDto>(items);
            return await query.ToListAsync();
        }

        public async Task<List<CModuleDto>> GetModulesPagesByCourseInstanceId(Guid courseInstanceId)
        {
            var courseInstance = await _ws.GetRepo<CourseInstance>().GetAsync(courseInstanceId);            
            if (courseInstance == null)
            {
                throw new UserFriendlyException("CourseInstance is not exist");
            }
            
            var courseId = courseInstance.CourseId;
            var query =
                from m in _ws.GetAll<Module>().Where(m => m.CourseId == courseId).OrderBy(m => m.SequenceOrder)
                join p in _ws.GetAll<Page, Guid>() on m.Id equals p.ModuleId into pages
                select new CModuleDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    SequenceOrder = m.SequenceOrder,
                    Pages = pages.OrderBy(p => p.SequenceOrder).Select(s => new CPageDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        SequenceOrder = s.SequenceOrder
                    })
                };
            //return new ListResultDto<CourseDto>(items);
            return await query.ToListAsync();
        }

        public async Task<List<CModuleDto>> GetModulesPagesForStudent(Guid courseInstanceId)
        {
            var studentId = AbpSession.UserId.Value;

            //var courseInstance = await _ws.GetRepo<CourseInstance>().GetAsync(courseInstanceId);
            var courseInstance = await _ws.GetRepo<CourseInstance>().GetAllIncluding(ci => ci.Course)
                .Where(ci => ci.Id == courseInstanceId)
                .Select(s => new { s.StartTime, s.EndTime, s.CourseId, s.Course.RestrictStudentFromViewThisCourseAfterEndDate, s.Course.RestrictStudentsFromViewingThisCourseBeforeEndDate })
                .FirstOrDefaultAsync();

            var now = DateTimeUtils.GetNow();
            if ((courseInstance.RestrictStudentFromViewThisCourseAfterEndDate && courseInstance.EndTime != null && courseInstance.EndTime.Value < now)
                || (courseInstance.RestrictStudentsFromViewingThisCourseBeforeEndDate && courseInstance.StartTime != null && courseInstance.StartTime.Value > now))
            {
                throw new UserFriendlyException("This course is not opening");
            }

            if (courseInstance == null)
            {
                throw new UserFriendlyException("CourseInstance is not exist");
            }
            var courseId = courseInstance.CourseId;

            // get pageIds that link to quiz or assignment
            var studentQuizPageIds = from scg in _ws.GetRepo<StudentCourseGroup>().GetAllIncluding(s => s.AssignedStudent).Where(s => s.AssignedStudent.StudentId == studentId)
                                     join gaq in _ws.GetRepo<GroupAssingedQuiz>().GetAllIncluding(s => s.QuizSetting).Where(s => s.QuizSetting.CourseInstanceId == courseInstanceId)
                                     on scg.CourseGroupId equals gaq.CourseGroupId
                                     join ple in _ws.GetRepo<PageLinkExam>().GetAllIncluding(x => x.Page).Where(s => s.LinkType != "assignment" && !s.Page.IsDeleted)
                                     on gaq.QuizSettingId equals ple.LinkId
                                     select ple.PageId;

            var quizPageIds = await studentQuizPageIds.Distinct().ToListAsync();
            var everyoneQuizPageIds = await (from gaq in _ws.GetRepo<GroupAssingedQuiz>().GetAllIncluding(s => s.CourseGroup, s => s.QuizSetting).Where(s => s.CourseGroup.IsEveryOne && s.QuizSetting.CourseInstanceId == courseInstanceId)
                                             join ple in _ws.GetAll<PageLinkExam>() on gaq.QuizSettingId equals ple.LinkId
                                             where !quizPageIds.Contains(ple.PageId)
                                             select ple.PageId).Distinct().ToListAsync();

            quizPageIds.AddRange(everyoneQuizPageIds);


            var assignmentQuery = from scg in _ws.GetRepo<StudentCourseGroup>().GetAllIncluding(s => s.AssignedStudent, s => s.CourseGroup).Where(s => s.AssignedStudent.StudentId == studentId || s.CourseGroup.IsEveryOne)
                                  join gaa in _ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseAssignment).Where(s => s.CourseAssignment.CourseInstanceId == courseInstanceId)
                                  on scg.CourseGroupId equals gaa.CourseGroupId
                                  join ple in _ws.GetAll<PageLinkExam>().Where(s => s.LinkType == "assignment")
                                  on gaa.AssignmentSettingId equals ple.LinkId
                                  select ple.PageId;

            var assignmentPageIds = await assignmentQuery.ToListAsync();

            var everyoneAssignmentPageIds = await (from gaq in _ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseGroup, s => s.CourseAssignment).Where(s => s.CourseGroup.IsEveryOne && s.CourseAssignment.CourseInstanceId == courseInstanceId)
                                                   join ple in _ws.GetAll<PageLinkExam>() on gaq.AssignmentSettingId equals ple.LinkId
                                                   where !assignmentPageIds.Contains(ple.PageId)
                                                   select ple.PageId).Distinct().ToListAsync();
            assignmentPageIds.AddRange(everyoneAssignmentPageIds);

            //get modules include pages
            var query =
               from m in _ws.GetAll<Module>().Where(m => m.CourseId == courseId).OrderBy(m => m.SequenceOrder)
               join pp in
                   from p in _ws.GetAll<Page, Guid>()
                   join ple in _ws.GetAll<PageLinkExam>() on p.Id equals ple.PageId into ples
                   select new
                   {
                       p.Id,
                       p.ModuleId,
                       p.Name,
                       p.SequenceOrder,
                       LinkType = !ples.Any() ? "page" : ples.FirstOrDefault().LinkType,
                       Type = p.Type
                   }
                on m.Id equals pp.ModuleId into pages
               select new CModuleDto
               {
                   Id = m.Id,
                   Name = m.Name,
                   SequenceOrder = m.SequenceOrder,
                   Pages = pages.Where(p => (p.Type == PageType.Page) || ((p.Type == PageType.Quiz || p.Type == PageType.Survey || p.Type == PageType.QuizFinal) && quizPageIds.Contains(p.Id)) || (p.Type == PageType.Assignment && assignmentPageIds.Contains(p.Id)))
                   .OrderBy(p => p.SequenceOrder).Select(s => new CPageDto
                   {
                       Id = s.Id,
                       Name = s.Name,
                       SequenceOrder = s.SequenceOrder,
                       LinkType = s.LinkType
                   })
               };
            var modulesPages = await query.ToListAsync();
            return modulesPages;

        }


        public async Task SaveModulesPages(ModulesPagesDto input)
        {
            var isExistCourse = await _ws.GetAll<Course, Guid>().AnyAsync(c => c.Id == input.CourseId);
            if (!isExistCourse) throw new UserFriendlyException(string.Format("The course id {0} is not exist", input.CourseId));
            if (input.Modules != null && !input.Modules.IsEmpty())
            {
                foreach (var module in input.Modules)
                {
                    var item = await _ws.GetRepo<Module>().GetAsync(module.Id);
                    item.SequenceOrder = module.SequenceOrder;
                    await _ws.GetRepo<Module>().UpdateAsync(item);

                    if (module.Pages != null && !module.Pages.IsEmpty())
                    {
                        foreach (var page in module.Pages)
                        {
                            var p = await _ws.GetRepo<Page>().GetAsync(page.Id);
                            p.SequenceOrder = page.SequenceOrder;
                            p.ModuleId = module.Id;
                            await _ws.GetRepo<Page>().UpdateAsync(p);
                        }
                    }
                }
            }

        }
        public async Task SavePages(CModulePagesDto input)
        {
            var isExistModule = await _ws.GetAll<Module, Guid>().AnyAsync(c => c.Id == input.ModuleId);
            if (!isExistModule) throw new UserFriendlyException(string.Format("The module id {0} is not exist", input.ModuleId));

            if (input.Pages != null && !input.Pages.IsEmpty())
            {
                foreach (var page in input.Pages)
                {
                    var p = await _ws.GetRepo<Page>().GetAsync(page.Id);
                    p.SequenceOrder = page.SequenceOrder;
                    await _ws.GetRepo<Page>().UpdateAsync(p);
                }
            }

        }
       
    }
}
