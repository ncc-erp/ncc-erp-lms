using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Assignments.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMALMS.DomainServices.Entity;
using RMALMS.DomainServices;
using RMALMS.Uitls;

namespace RMALMS.Assignments
{
    [AbpAuthorize]
    public class AssignmentAppService : AsyncCrudAppService<Assignment, AssignmentDto, Guid, PagedResultRequestDto, CreateAssignmentDto, AssignmentDto>, IAssignmentAppService
    {
        private readonly IWorkScope _ws;
        private readonly IUserCertificationManager _userCertificationManager;
        private readonly ICourseManager _courseManager;
        public AssignmentAppService(
            IRepository<Assignment, Guid> repository,
            IWorkScope workScope,
            IUserCertificationManager userCerfiticationManager,
            ICourseManager courseManager
            ) : base(repository)
        {
            _ws = workScope;
            this._userCertificationManager = userCerfiticationManager;
            this._courseManager = courseManager;
        }

        public async Task<List<AssignmentDto>> GetAssignmentsByCourseId(Guid courseId)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == courseId).ProjectTo<AssignmentDto>();
            return await query.ToListAsync();
        }

        public async Task<List<SelectAssignmentDto>> GetSelectAssignmentsByCourseInstanceId(Guid courseInstanceId)
        {
            var query = _ws.GetRepo<AssignmentSetting>().GetAllIncluding(s => s.Assignment).Where(p => p.CourseInstanceId == courseInstanceId && p.Assignment.Status == CourseAssigmentStatus.Publish)
                .Select(s => new SelectAssignmentDto { Id = s.Id, Title = s.Assignment.Title });
            return await query.ToListAsync();
        }

        public async override Task<AssignmentDto> Update(AssignmentDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            List<string> notifymessage = new List<string>();
            if (item.Title != input.Title)
            {
                notifymessage.Add("<p>Title has changed from '" + item.Title + "' to '" + input.Title + "' </p>");
            }
            if (item.Content != input.Content)
            {
                notifymessage.Add("<p>Content has changed </p>");
            }
            //if (item.Point != input.Point)
            //{
            //    notifymessage.Add("<p>Point has changed from '" + (item.Point != null ? item.Point.ToString() : "none set") + "' to '" + (input.Point != null ? input.Point.ToString() : "none set") + "'</p>");
            //}
            if (item.DisplayGrade != input.DisplayGrade)
            {
                notifymessage.Add("<p>Display grade has changed from '" + (DisplayGradeType)item.DisplayGrade + "' to '" + (DisplayGradeType)input.DisplayGrade + "' </p>");
            }
            if (item.SubmissionType != input.SubmissionType)
            {
                notifymessage.Add("<p>Response Type has changed from '" + (SubmissionType)item.SubmissionType + "' to '" + (SubmissionType)input.SubmissionType + "'</p>");
            }

            MapToEntity(input, item);
            await Repository.UpdateAsync(item);
            var itemDto = MapToEntityDto(item);

            var setting = await _ws.GetRepo<AssignmentSetting>().GetAsync(input.settings.Id);

            notifymessage.Add("<p>Setting has been changed</p>");
            ObjectMapper.Map<AssignmentSettingsDto, AssignmentSetting>(input.settings, setting);
            await _ws.UpdateAsync(setting);


            var oldgroupassign = (from gaq in _ws.GetAll<GroupAssingedAssignment, Guid>()
                                  where gaq.AssignmentSettingId == setting.Id
                                  select gaq.CourseGroupId).ToList();
            var newgroupassign = input.GroupsAssingedAssignment.FindAll(n => !oldgroupassign.Contains(n));
            foreach (var group in newgroupassign)
            {
                var groupasign = new GroupAssingedAssignment
                {
                    CourseGroupId = group,
                    AssignmentSettingId = setting.Id
                };
                await _ws.InsertAndGetIdAsync(groupasign);
            }
            var deleteitem = oldgroupassign.FindAll(o => !input.GroupsAssingedAssignment.Contains(o));
            if (deleteitem != null)
                await _ws.GetRepo<GroupAssingedAssignment>().DeleteAsync(ga => (ga.AssignmentSettingId == input.settings.Id && deleteitem.Contains(ga.CourseGroupId)));
            if (newgroupassign.Count > 0 || deleteitem.Count > 0)
                notifymessage.Add("<p>Assigned group has been changed</p>");
            if (input.AllowNotify && notifymessage.Count > 0)
            {
                var notify = new Annoucement();
                notify.Title = "Update assignment '" + item.Title + "'";
                notify.Content = string.Join("\n", notifymessage);
                notify.CourseInstanceId = input.settings.CourseInstanceId;
                await _ws.InsertAndGetIdAsync(notify);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            //itemDto.settings = _ws.GetAll<AssignmentSetting, Guid>().Where(s => s.Id == setting.Id).ProjectTo<AssignmentSettingsDto>().FirstOrDefault();
            itemDto.settings = ObjectMapper.Map<AssignmentSettingsDto>(setting);

            return itemDto;
        }

        public async override Task<AssignmentDto> Create(CreateAssignmentDto input)
        {
            var isExistCourse = await _ws.GetAll<Course, Guid>().AnyAsync(c => c.Id == input.CourseId);
            if (!isExistCourse)
                throw new UserFriendlyException(string.Format("The course id {0} is not exist", input.CourseId));
            var item = ObjectMapper.Map<Assignment>(input);
            item.Id = await _ws.InsertAndGetIdAsync(item);
            var itemDto = MapToEntityDto(item);

            var setting = ObjectMapper.Map<AssignmentSetting>(input.settings);
            setting.AssingmentId = item.Id;
            setting.Id = await _ws.InsertAndGetIdAsync(setting);
            foreach (var groupId in input.GroupsAssingedAssignment)
            {
                var groupasign = new GroupAssingedAssignment();
                groupasign.CourseGroupId = groupId;
                groupasign.AssignmentSettingId = setting.Id;
                await _ws.InsertAndGetIdAsync(groupasign);
            }
            if (input.AllowNotify)
            {
                var notify = new Annoucement();
                notify.Title = "Create Assignment '" + item.Title + "'";
                notify.Content = "New Assignment has been added";
                notify.CourseInstanceId = input.settings.CourseInstanceId;
                await _ws.InsertAndGetIdAsync(notify);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            //itemDto.settings = _ws.GetAll<AssignmentSetting, Guid>().Where(s => s.Id == setting.Id).ProjectTo<AssignmentSettingsDto>().FirstOrDefault();
            itemDto.settings = ObjectMapper.Map<AssignmentSettingsDto>(setting);

            return itemDto;
        }
        public async override Task<AssignmentDto> Get(EntityDto<Guid> input)
        {
            var item = await Repository.GetAsync(input.Id);
            var itemDto = MapToEntityDto(item);
            var courseintance = await _ws.GetRepo<CourseInstance>().GetAll().Where(ci => ci.CourseId == item.CourseId && ci.Status == CourseSettingStatus.Active).FirstOrDefaultAsync();
            if (courseintance != null)
            {
                itemDto.CourseInstanceId = courseintance.Id;
                var setting = await _ws.GetAll<AssignmentSetting>().Where(a => a.AssingmentId == item.Id && a.CourseInstanceId == courseintance.Id).ProjectTo<AssignmentSettingsDto>().FirstOrDefaultAsync();
                if (setting == null)
                    setting = new AssignmentSettingsDto();
                itemDto.settings = setting;
            }
            return itemDto;

        }

        public async Task<AssignmentDto> GetByAssignmentSettingId(EntityDto<Guid> input)
        {
            var setting = await _ws.GetRepo<AssignmentSetting>().GetAsync(input.Id);
            var item = await Repository.GetAsync(setting.AssingmentId);
            var itemDto = MapToEntityDto(item);
            if ((setting.StartTimeUtc ?? DateTime.MinValue) > DateTimeUtils.GetNow() || (setting.EndTimeUtc ?? DateTime.MaxValue) < DateTimeUtils.GetNow())
            {
                itemDto = new AssignmentDto();
                itemDto.IsDisable = true;
            }
           
            itemDto.CourseInstanceId = setting.CourseInstanceId;
            itemDto.settings = ObjectMapper.Map<AssignmentSettingsDto>(setting);
            return itemDto;
        }

        [HttpPost]
        public async Task<GridResult<AssignmentDto>> GetAssignmentsByCourseIdPagging(GridAssignmentsParam input)
        {
            var query = Repository.GetAll().Where(p => p.CourseId == input.courseId).ProjectTo<AssignmentDto>();

            var result = await query.GetGridResult(query, input.input);
            foreach (var item in result.Items)
            {
                var setting = _ws.GetAll<AssignmentSetting, Guid>()
                    .Where(a => a.AssingmentId == item.Id && a.CourseInstanceId == input.courseInstanceId).ProjectTo<AssignmentSettingsDto>().FirstOrDefault();
                if (setting != null)
                {
                    item.settings = setting;
                    var groupAssign = _ws.GetAll<GroupAssingedAssignment, Guid>().Where(g => g.AssignmentSettingId == setting.Id).ToList();
                    var listIds = new List<Guid>();
                    foreach (var gra in groupAssign)
                    {
                        listIds.Add(gra.CourseGroupId);
                    }
                    item.GroupsAssingedAssignment = listIds;
                }
            }

            return result;
        }

        public async Task<List<StudentAssignmentDto>> UpdateStudentAssignmentScore(List<StudentAssignmentDto> inputs)
        {
            var result = new List<StudentAssignmentDto>();
            if (inputs.Count() == 0)
            {
                return result;
            }
            
            var assignSetting = await _ws.GetRepo<AssignmentSetting>().GetAsync(inputs[0].AssignmentSettingId);
            if (assignSetting == null)
            {
                throw new UserFriendlyException(String.Format("AssignmentSetting id {0} is not exist ", inputs[0].AssignmentSettingId));
            }
            if (inputs[0].Point > assignSetting.Point)
            {
                throw new UserFriendlyException(String.Format("Student Assignment Score cannot greater than {0} ", assignSetting.Point));
            }
            var assignment = await _ws.GetRepo<Assignment>().GetAsync(assignSetting.AssingmentId);
            if (assignment == null)
            {
                throw new UserFriendlyException(String.Format("Assignment id {0} is not exist ", assignSetting.AssingmentId));
            }
            foreach (var input in inputs)
            {
                if (input.Id == null)
                {
                    var item = await _ws.GetAll<StudentAssignment>().Where(s => s.CourseAssignedStudentId == input.CourseAssignedStudentId && s.AssignmentSettingId == input.AssignmentSettingId).LastOrDefaultAsync();
                    if (item != null)
                    {
                        input.Id = item.Id;
                    }
                }

                if (input.Id != null && input.Id != Guid.Empty)
                {
                    //update
                    if (assignment.IsGroupAssignment && !assignment.IsAssignIndividualGrade && input.IsApplyForAllStudentInGroup)
                    {
                        //apply score for all student in course group
                        var query = from gaa in _ws.GetAll<GroupAssingedAssignment>().Where(s => s.AssignmentSettingId == input.AssignmentSettingId)
                                    join scg in _ws.GetAll<StudentCourseGroup>()
                                    on gaa.CourseGroupId equals scg.CourseGroupId into students
                                    where students.Select(s => s.AssignedStudentId).Contains(input.CourseAssignedStudentId)
                                    from s in students
                                    select s.AssignedStudentId;
                        var courseAssignedStudentIds = await query.ToListAsync();
                        foreach (var courseAssignedStudentId in courseAssignedStudentIds)
                        {
                            var sa = await _ws.GetAll<StudentAssignment>().Where(s => s.AssignmentSettingId == input.AssignmentSettingId && s.CourseAssignedStudentId == courseAssignedStudentId)
                                .FirstOrDefaultAsync();
                            if (sa != null)
                            {
                                sa.Point = input.Point;
                                await _ws.UpdateAsync(sa);
                            }
                            else
                            {
                                sa = new StudentAssignment
                                {
                                    CourseAssignedStudentId = courseAssignedStudentId,
                                    AssignmentSettingId = input.AssignmentSettingId,
                                    Point = input.Point
                                };
                                sa.Id = await _ws.InsertAndGetIdAsync(sa);
                            }


                            result.Add(ObjectMapper.Map<StudentAssignmentDto>(sa));
                        }

                    }
                    else
                    {
                        //update for student only
                        var sa = await _ws.GetRepo<StudentAssignment>().GetAsync(input.Id);
                        ObjectMapper.Map<StudentAssignmentDto, StudentAssignment>(input, sa);
                        await _ws.UpdateAsync(sa);
                        result.Add(ObjectMapper.Map<StudentAssignmentDto>(sa));
                    }
                }
                else
                {
                    //insert for first time
                    if (assignment.IsGroupAssignment && !assignment.IsAssignIndividualGrade)
                    {
                        //students in course groups do assignment together
                        //=> 
                        var query = from gaa in _ws.GetRepo<GroupAssingedAssignment>().GetAllIncluding(s => s.CourseAssignment).Where(s => s.AssignmentSettingId == input.AssignmentSettingId)                                    
                                    join scg in _ws.GetRepo<StudentCourseGroup>().GetAllIncluding(s => s.AssignedStudent)
                                    on gaa.CourseGroupId equals scg.CourseGroupId into students
                                    where students.Select(s => s.AssignedStudentId).Contains(input.CourseAssignedStudentId)
                                    from s in students
                                    select s.AssignedStudentId;
                        var courseAssignedStudentIds = await query.ToListAsync();
                        foreach (var courseAssignedStudentId in courseAssignedStudentIds)
                        {
                            var sa = new StudentAssignment
                            {
                                CourseAssignedStudentId = courseAssignedStudentId,
                                AssignmentSettingId = input.AssignmentSettingId,
                                Point = input.Point
                            };
                            sa.Id = await _ws.InsertAndGetIdAsync(sa);
                            result.Add(ObjectMapper.Map<StudentAssignmentDto>(sa));
                        }
                    }
                    else
                    {
                        //students do assignment one by one
                        var sa = new StudentAssignment
                        {
                            CourseAssignedStudentId = input.CourseAssignedStudentId,
                            AssignmentSettingId = input.AssignmentSettingId,
                            Point = input.Point
                        };
                        sa.Id = await _ws.InsertAndGetIdAsync(sa);
                        result.Add(ObjectMapper.Map<StudentAssignmentDto>(sa));
                    }
                }
            }
            if (result.Count() > 0)
            {
                CurrentUnitOfWork.SaveChanges();
                var courseInstanceId = assignSetting.CourseInstanceId;
                foreach (var courseAssignedStudentId in result.Select(s => s.CourseAssignedStudentId).Distinct())
                {
                    await _userCertificationManager.CreateUpdateUserCertification(courseAssignedStudentId, CertificationType.Completion, UpdateUserCertificationOption.UpdateOnly);
                }
            }

            return result;
        }
        public async Task<List<StudentQuizAssignmentScore>> GetStudentAssignmentProgress(Guid courseInstanceId)
        {
            var userId = AbpSession.UserId.Value;
            var courseAssignedStudentId = (await _ws.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == userId).LastOrDefaultAsync()).Id;
            return await _courseManager.GetStudentAssignmentScores(courseAssignedStudentId, courseInstanceId);
        }
    }
}
