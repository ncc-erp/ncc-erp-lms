using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Entities;
using RMALMS.IoC;
using RMALMS.Pages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Pages
{
    [AbpAuthorize]
    public class PageAppService : AsyncCrudAppService<Page, PageDto, Guid, PagedResultRequestDto, CreatePageDto, PageDto>, IPageAppService
    {
        private readonly IWorkScope _ws;
        public PageAppService(IRepository<Page, Guid> repository, IWorkScope workScope) : base(repository)
        {
            _ws = workScope;
        }

        public async Task<ListResultDto<PagesByModuleDto>> getPagesByModuleId(Guid moduleId)
        {
            var query = Repository.GetAll().Where(p => p.ModuleId == moduleId).OrderBy(p => p.SequenceOrder).ProjectTo<PagesByModuleDto>();
            var items = await query.ToListAsync();
            return new ListResultDto<PagesByModuleDto>(items);
        }

        public async Task<ListResultDto<PagesByCourseDto>> getPagesByCourseId(Guid courseId)
        {
            var query = Repository.GetAllIncluding(p => p.Module).Where(p => p.CourseId == courseId).Select(p =>
                new PagesByCourseDto { Id = p.Id, Name = p.Name, ModuleName = p.Module.Name, ModuleId = p.ModuleId }
            );
            var items = await query.ToListAsync();
            return new ListResultDto<PagesByCourseDto>(items);
        }

        public async override Task<PageDto> Update(PageDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            //MapToEntity(input, item);
            ObjectMapper.Map<PageDto, Page>(input, item);
            if (input.Links == null || input.Links.Count() == 0)
            {
                item.Type = PageType.Page;
            }
            else
            {
                if (input.Links[0].LinkType.ToLower().Trim() == "quiz")
                {
                    item.Type = PageType.Quiz;
                }
                else if (input.Links[0].LinkType.ToLower().Trim() == "quiz_final")
                {
                    item.Type = PageType.QuizFinal;
                }
                else if (input.Links[0].LinkType.ToLower().Trim() == "survey")
                {
                    item.Type = PageType.Survey;
                }
                else
                {
                    item.Type = PageType.Assignment;
                }
            }

            await Repository.UpdateAsync(item);

            if (input.Links != null)
            {
                //current
                var currentListId = await _ws.GetAll<PageLinkExam>().Where(s => s.PageId == input.Id).Select(s => s.Id).ToListAsync();

                //insert
                var insertList = input.Links.Where(s => !currentListId.Contains(s.Id));
                foreach (var pleDto in insertList)
                {
                    var ple = ObjectMapper.Map<PageLinkExam>(pleDto);
                    ple.PageId = input.Id;
                    await _ws.InsertAsync(ple);
                }

                //update
                var updateList = input.Links.Where(s => currentListId.Contains(s.Id));
                foreach (var dto in updateList)
                {
                    var ple = ObjectMapper.Map<PageLinkExam>(dto);
                    ple.PageId = item.Id;
                    await _ws.UpdateAsync(ple);
                }

                //delete
                var updatedListId = updateList.Select(s => s.Id);
                var deleteListId = currentListId.Where(s => !updatedListId.Contains(s));
                foreach (var id in deleteListId)
                {
                    await _ws.GetRepo<PageLinkExam>().DeleteAsync(id);

                }
            }
            return ObjectMapper.Map<PageDto>(item);
        }


        public async override Task<PageDto> Create(CreatePageDto input)
        {
            var item = ObjectMapper.Map<Page>(input);
            var courseId = _ws.GetRepo<Module>().Get(input.ModuleId).CourseId;
            item.CourseId = courseId;
            item.Identifier = DateTime.Now.ToFileTimeUtc().ToString();
            item.SequenceOrder = _ws.GetAll<Page, Guid>().Count(p => p.ModuleId == input.ModuleId);
            if (input.Links == null || input.Links.Count() == 0)
            {
                item.Type = PageType.Page;
            }
            else
            {
                if (input.Links[0].LinkType.ToLower().Trim() == "quiz")
                {
                    item.Type = PageType.Quiz;
                }
                else if (input.Links[0].LinkType.ToLower().Trim() == "quiz_final")
                {
                    item.Type = PageType.QuizFinal;
                }
                else if (input.Links[0].LinkType.ToLower().Trim() == "survey")
                {
                    item.Type = PageType.Survey;
                }
                else
                {
                    item.Type = PageType.Assignment;
                }
            }

            item.Id = await _ws.InsertAndGetIdAsync(item);

            //page link exam - link to assignment or/and quiz
            if (input.Links != null)
            {
                foreach (PageLinkExamDto pleDto in input.Links)
                {
                    var ple = ObjectMapper.Map<PageLinkExam>(pleDto);
                    ple.PageId = item.Id;
                    await _ws.InsertAsync(ple);
                }
            }

            return ObjectMapper.Map<PageDto>(item);
        }

        public async override Task<PageDto> Get(EntityDto<Guid> input)
        {
            PageDto page = await base.Get(input);
            var links = await _ws.GetAll<PageLinkExam>().Where(ple => ple.PageId == input.Id).OrderBy(s => s.Id).Select(s => new PageLinkExamDto
            {
                Id = s.Id,
                LinkId = s.LinkId,
                LinkType = s.LinkType,
                SequenceOrder = s.SequenceOrder
            }).ToListAsync();
            page.Links = links;
            return page;
        }
        public async Task<PageDto> GetForStudent(EntityDto<Guid> input)
        {
            PageDto page = await base.Get(input);
            var studentId = AbpSession.UserId.Value;
            var linkType = await _ws.GetAll<PageLinkExam>().Where(ple => ple.PageId == input.Id).OrderBy(s => s.LinkId).Select(s => s.LinkType).FirstOrDefaultAsync();

            if (linkType == null)
            {
                page.Links = new List<PageLinkExamDto>();
            }
            else if (page.Type == PageType.Quiz || page.Type == PageType.QuizFinal || page.Type == PageType.Survey)
            {
                var studentQuizIds = from scg in _ws.GetRepo<StudentCourseGroup>().GetAllIncluding(s => s.AssignedStudent).Where(s => s.AssignedStudent.StudentId == studentId)
                                     join gaq in _ws.GetRepo<GroupAssingedQuiz>().GetAllIncluding(s => s.QuizSetting)
                                     on scg.CourseGroupId equals gaq.CourseGroupId
                                     select gaq.QuizSettingId;


                var links = await (from ple in _ws.GetAll<PageLinkExam>().Where(s => s.PageId == input.Id)
                                   join squizId in studentQuizIds on ple.LinkId equals squizId into quizs
                                   join x in
                                       from gaq in _ws.GetAll<GroupAssingedQuiz>()
                                       join cg in _ws.GetAll<CourseGroup>().Where(s => s.IsEveryOne) on gaq.CourseGroupId equals cg.Id
                                       select gaq.QuizSettingId
                                   on ple.LinkId equals x into everyoneGroup
                                   let otherGroup = quizs.Any()
                                   let orderPoint = otherGroup ? 1 : 0
                                   where everyoneGroup.Any() || otherGroup
                                   select new PageLinkExamDto
                                   {
                                       Id = ple.Id,
                                       LinkId = ple.LinkId,
                                       LinkType = ple.LinkType,
                                       SequenceOrder = orderPoint

                                   }).OrderByDescending(s => s.SequenceOrder).ThenBy(s => s.LinkId).ToListAsync();
                page.Links = links;
            }
            else if (page.Type == PageType.Assignment)
            {
                var gquery = _ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseAssignment).Where(s => (s.CourseAssignment.StartTimeUtc ?? DateTime.MinValue) < DateTime.Now && (s.CourseAssignment.EndTimeUtc ?? DateTime.MaxValue) > DateTime.Now);
                var studentAssingmentIds = from scg in _ws.GetRepo<StudentCourseGroup>().GetAllIncluding(s => s.AssignedStudent).Where(s => s.AssignedStudent.StudentId == studentId)
                                           join gaq in _ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseAssignment)
                                           //_ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseAssignment).Where(s=>(s.CourseAssignment.StartTimeUtc ?? DateTime.MinValue) < DateTime.Now && ( s.CourseAssignment.EndTimeUtc ?? DateTime.MaxValue) > DateTime.Now  )
                                           on scg.CourseGroupId equals gaq.CourseGroupId
                                           select gaq.AssignmentSettingId;


                var links = await (from ple in _ws.GetAll<PageLinkExam>().Where(s => s.PageId == input.Id)
                                   join assignmentId in studentAssingmentIds on ple.LinkId equals assignmentId into assignments
                                   join x in
                                       from gaq in _ws.GetAll<GroupAssingedAssignment>()
                                       join cg in _ws.GetAll<CourseGroup>().Where(s => s.IsEveryOne) on gaq.CourseGroupId equals cg.Id
                                       select gaq.AssignmentSettingId
                                   on ple.LinkId equals x into everyoneGroup
                                   let otherGroup = assignments.Any()
                                   let orderPoint = otherGroup ? 1 : 0
                                   where everyoneGroup.Any() || otherGroup
                                   select new PageLinkExamDto
                                   {
                                       Id = ple.Id,
                                       LinkId = ple.LinkId,
                                       LinkType = ple.LinkType,
                                       SequenceOrder = orderPoint
                                   }).OrderByDescending(s => s.SequenceOrder).ToListAsync();
                page.Links = links;
                var assignmentsetting = _ws.GetRepo<AssignmentSetting>().Get(links[0].LinkId);
                if ((assignmentsetting.StartTimeUtc ?? DateTime.MinValue) < DateTime.Now && (assignmentsetting.EndTimeUtc ?? DateTime.MaxValue) > DateTime.Now)
                {
                    var assignment = _ws.GetRepo<Assignment>().Get(assignmentsetting.AssingmentId);
                    var queryfiles = from r in _ws.GetAll<Resource>()
                                     where r.EntityType == "assignment" && r.EntityId == assignment.Id
                                     select new CFileDto
                                     {
                                         FileName = r.FileName,
                                         FilePath = r.FilePath
                                     };
                    page.Files = queryfiles.ToList();
                }
            }

            //bookmark
            page.Bookmarked = await _ws.GetAll<UserBookMark>().AnyAsync(x => x.PageId == page.Id);
            return page;
        }


        #region Student-BookMark
        public async Task<BookMarkInputDto> BookmarkPage(BookMarkInputDto input)
        {

            var item = await _ws.GetAll<UserBookMark>().FirstOrDefaultAsync(m => m.CreatorUserId == AbpSession.UserId && m.CourseInstanceId == input.CourseInstanceId && m.PageId == input.PageId);
            if (item != null)
            {
                input.Id = item.Id;
                return input;
            }
            else
            {
                var qBookMark = new UserBookMark
                {
                    PageId = input.PageId,
                    CourseInstanceId = input.CourseInstanceId,
                };
                input.Id = await _ws.InsertAndGetIdAsync(qBookMark);
                return input;
            }
        }

        [HttpDelete]
        public async Task UnBookmarkPage(EntityDto<Guid> pageId)
        {
            await _ws.GetRepo<UserBookMark>().DeleteAsync(s => s.PageId == pageId.Id && s.CreatorUserId == AbpSession.UserId);
        }

        public async Task<ListResultDto<BookMarkOut>> GetsUserBookMark(Guid courseInstanceId)
        {
            var qBookMark = from bm in _ws.GetRepo<UserBookMark, Guid>().GetAll()
                            .Where(m => m.CreatorUserId == AbpSession.UserId && m.CourseInstanceId == courseInstanceId)
                            join page in _ws.GetRepo<Page, Guid>().GetAll()
                            on bm.PageId equals page.Id

                            join module in _ws.GetRepo<Module, Guid>().GetAll()
                            on page.ModuleId equals module.Id

                            select new BookMarkOut
                            {
                                Id = bm.Id,
                                ModuleId = module.Id,
                                ModuleName = module.Name,
                                PageId = page.Id,
                                PageName = page.Name,
                                CreationTime = bm.CreationTime
                            };
            var result = await qBookMark.ToListAsync();
            return new ListResultDto<BookMarkOut>(result);
        }

        #endregion
    }
}
