using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Users;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using RMALMS.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using RMALMS.DomainServices;

namespace RMALMS.Courses
{
    public class CourseGroupAppService : CrudApplicationBaseService<CourseGroup, CourseGroupDto, Guid, PagedResultRequestDto, CourseGroupDto, CourseGroupDto>
    {
        private readonly IWorkScope _ws;
        private readonly ICourseManager _courseManager;
        public CourseGroupAppService(IRepository<CourseGroup, Guid> respository, IWorkScope ws, ICourseManager courseManager)
            : base(respository)
        {
            _ws = ws;
            _courseManager = courseManager;
        }

        public async override Task<CourseGroupDto> Create(CourseGroupDto input)
        {
            var item = ObjectMapper.Map<CourseGroup>(input);
            //item.Id = Guid.Empty;
            var isExist = await _ws.GetAll<CourseGroup>().AnyAsync(cg => cg.CourseInstanceId == input.CourseInstanceId && cg.Name == input.Name);
            if (isExist) throw new UserFriendlyException(String.Format("Duplicate group name {0}", input.Name));
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<CourseGroupDto>(item);
        }

        public async Task<List<CourseGroupWithMemberDto>> GetAllCourseGroupByCourse(Guid courseInstanceId)
        {
            var qcourseGroups =
                from cg in Repository.GetAll().Where(c => c.CourseInstanceId == courseInstanceId && !c.IsEveryOne)
                join u in
                    from sc in WorkScope.GetAll<StudentCourseGroup>()
                    join assignedstudent in WorkScope.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId) on sc.AssignedStudentId equals assignedstudent.Id
                    join u in WorkScope.GetAll<User, long>() on assignedstudent.StudentId equals u.Id
                    select new { sc.Id, u.DisplayName, u.UserName, AssignedStudentId = assignedstudent.Id, sc.CourseGroupId, assignedstudent.EnrollCount, assignedstudent.Status }
                on cg.Id equals u.CourseGroupId into students
                select new CourseGroupWithMemberDto
                {
                    GroupName = cg.Name,
                    Id = cg.Id,
                    Students = students.Select(
                        s => new StudentCourseGroupListDto
                        {
                            AssignedStudentId = s.AssignedStudentId,
                            Id = s.Id,
                            StudentName = s.DisplayName == null || s.DisplayName == "" ? s.UserName : s.DisplayName,
                            EnrollCount = s.EnrollCount,
                            Status = s.Status
                        })
                };
            return await qcourseGroups.ToListAsync();
        }
        public async Task<List<CourseGroupWithMemberDto>> GetCourseGroupsByCourseId(Guid courseInstanceId)
        {
            var qcourseGroups =
                from cg in Repository.GetAll().Where(c => c.CourseInstanceId == courseInstanceId)
                select new CourseGroupWithMemberDto
                {
                    GroupName = cg.Name,
                    Id = cg.Id,
                    Selected = false,
                    IsEveryOne= cg.IsEveryOne
                };
            return await qcourseGroups.ToListAsync();
        }
        public async Task<List<StudentCourseGroupListDto>> GetUnAssignedStudents(Guid courseInstanceId)
        {
            var qusers =
                //from assignedstudent in WorkScope.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId && s.Status == AssignedStatus.Accepted)
                from assignedstudent in this._courseManager.GetCourseAssignedStudentsAcceptedAndCompleted(courseInstanceId)
                join sc in WorkScope.GetAll<StudentCourseGroup>() on assignedstudent.Id equals sc.AssignedStudentId into groups
                join u in WorkScope.GetAll<User, long>() on assignedstudent.StudentId equals u.Id
                where !groups.Any()
                select new
                StudentCourseGroupListDto
                {
                    StudentName = u.DisplayName == null || u.DisplayName == "" ? u.UserName : u.DisplayName,
                    AssignedStudentId = assignedstudent.Id,
                    EnrollCount = assignedstudent.EnrollCount,
                    Status = assignedstudent.Status
                };
            //
            return await qusers.ToListAsync();
        }

        public async override Task<CourseGroupDto> Update(CourseGroupDto input)
        {
            var isExist = await _ws.GetAll<CourseGroup>().AnyAsync(cg => cg.CourseInstanceId == input.CourseInstanceId && cg.Name == input.Name && cg.Id != input.Id);
            if (isExist) throw new UserFriendlyException(String.Format("Duplicate group name {0}", input.Name));
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }

        public async Task SaveStudentCourseGroups(List<StudentCourseGroupDto> inputs)
        {
            foreach (var item in inputs)
            {
                var alreadyList = _ws.GetAll<StudentCourseGroup>().Where(scg => scg.CourseGroupId == item.CourseGroupId).Select(s => s.AssignedStudentId);
                //insert
                var insertList = item.AssignedStudentIds.Except(alreadyList);
                foreach (var assignedStudentId in insertList)
                {
                    var obj = new StudentCourseGroup()
                    {
                        AssignedStudentId = assignedStudentId,
                        CourseGroupId = item.CourseGroupId,
                    };
                    await _ws.InsertAsync<StudentCourseGroup>(obj);
                }

                //delete
                var deleteList = alreadyList.Except(item.AssignedStudentIds);
                foreach (var assignedStudentId in deleteList)
                {
                    await _ws.GetRepo<StudentCourseGroup>().DeleteAsync(s => s.AssignedStudentId == assignedStudentId && s.CourseGroupId == item.CourseGroupId);
                }
            }
        }
        public async Task<List<CourseGroupWithMemberDto>> GetAllCourseGroupByQuiz(Guid quizId)
        {
            try
            {

                var quizsetting = (from q in WorkScope.GetAll<QuizSetting>()
                                   join ci in WorkScope.GetAll<CourseInstance>() on q.CourseInstanceId equals ci.Id
                                   where q.QuizId == quizId && ci.Status == CourseSettingStatus.Active
                                   select q).FirstOrDefault();
                var qcourseGroups =
                     from cg in Repository.GetAll()
                     join gaq in WorkScope.GetAll<GroupAssingedQuiz>().Where(g => g.QuizSettingId == quizsetting.Id)
                     on cg.Id equals gaq.CourseGroupId into mappings
                     from mapping in mappings.DefaultIfEmpty()
                     where cg.CourseInstanceId == quizsetting.CourseInstanceId
                     select new CourseGroupWithMemberDto
                     {
                         GroupName = cg.Name,
                         Id = cg.Id,
                         Selected = mapping.QuizSettingId == quizsetting.Id
                     };
                return await qcourseGroups.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(string.Format("Data not valid"));
            }
        }
        public async Task<List<CourseGroupWithMemberDto>> GetAllCourseGroupByAssignment(Guid assignmentId)
        {
            try
            {
                var assignment = WorkScope.GetAll<Assignment>().Where(a => a.Id == assignmentId).FirstOrDefault();
                if (assignment == null)
                    throw new UserFriendlyException(string.Format("Assignment not exist"));

                var assignmentsetting = (from q in WorkScope.GetAll<AssignmentSetting>()
                                         join ci in WorkScope.GetAll<CourseInstance>() on q.CourseInstanceId equals ci.Id
                                         where q.AssingmentId == assignmentId && ci.Status == CourseSettingStatus.Active
                                         select q).FirstOrDefault();
                var qcourseGroups =
                     from cg in Repository.GetAll()
                     join gaq in WorkScope.GetAll<GroupAssingedAssignment>().Where(g => g.AssignmentSettingId == assignmentsetting.Id)
                     on cg.Id equals gaq.CourseGroupId
                     into mappings
                     from mapping in mappings.DefaultIfEmpty()
                     where cg.CourseInstanceId == assignmentsetting.CourseInstanceId
                     select new CourseGroupWithMemberDto
                     {
                         GroupName = cg.Name,
                         Id = cg.Id,
                         Selected = mapping.AssignmentSettingId == assignmentsetting.Id
                     };
                return await qcourseGroups.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(string.Format("Data not valid"));
            }
        }

    }
}
