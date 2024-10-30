using Abp.Authorization;
using Abp.Configuration;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RMALMS.Configuration;
using RMALMS.Courses.Dto;
using RMALMS.Entities;
using RMALMS.Groups.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.Courses
{
    [AbpAuthorize]
    public class UserAssignedToCourses : ApplicationBaseService
    {
        #region Group Assign To Course
        public async Task<List<BrifGroupDto>> GetGroupsAssignedToCourse(Guid courseInstanceId)
        {
            return await _ws.GetRepo<GroupAssignedCourse>().GetAllIncluding(gac => gac.Group)
                .Where(s => s.CourseInstanceId == courseInstanceId)
                .Select(s => new BrifGroupDto()
                {
                    Id = s.GroupId,
                    Name = s.Group.Name
                })
                .ToListAsync();
        }

        public async Task AddGroupsToCourse(GroupsToCourseDto input)
        {
            //Check course existing
            var isExistCourse = await _ws.GetRepo<CourseInstance>().GetAll().AnyAsync(u => u.Id == input.CourseInstanceId);

            if (!isExistCourse) throw new UserFriendlyException(string.Format("The course instance id {0} is not exist", input.CourseInstanceId));

            var alreadyList = await _ws.GetRepo<GroupAssignedCourse>().GetAll().Where(ug => ug.CourseInstanceId == input.CourseInstanceId).Select(ug => ug.GroupId).ToListAsync();

            //insert
            var insertList = input.Groups.Except(alreadyList);
            foreach (var groupId in insertList)
            {
                var item = new GroupAssignedCourse
                {
                    CourseInstanceId = input.CourseInstanceId,
                    GroupId = groupId
                };
                await _ws.InsertAsync<GroupAssignedCourse>(item);
            }

            //delete
            var deleteList = alreadyList.Except(input.Groups);
            await _ws.GetRepo<GroupAssignedCourse>().DeleteAsync(ug => deleteList.Contains(ug.GroupId) && ug.CourseInstanceId == input.CourseInstanceId);
            await CurrentUnitOfWork.SaveChangesAsync();

        }
        #endregion

        #region Student Assign To Course
        public async Task<List<CourseAssignedStudentDto>> GetStudentAssignedToCourse(Guid courseInstanceId)
        {
            return await _ws.GetAll<CourseAssignedStudent>()
                .Where(s => s.CourseInstanceId == courseInstanceId)
                .ProjectTo<CourseAssignedStudentDto>()
                .ToListAsync();
        }

        public async Task<List<long>> AddStudentsToCourse(StudentsToCourseDto input)
        {
            var isExistingCourse = await _ws.GetRepo<CourseInstance>().GetAll().AnyAsync(ci => ci.Id == input.CourseInstanceId);
            if (!isExistingCourse) throw new UserFriendlyException(string.Format("The course instance id {0} is not exist", input.CourseInstanceId));

            var CASByCourses = _ws.GetAll<CourseAssignedStudent>().Where(cas => cas.CourseInstanceId == input.CourseInstanceId);

            var alreadyList = await CASByCourses.Select(s => s.StudentId).ToListAsync();
            //insert
            var insertList = input.Students.Except(alreadyList);
            foreach (var studentId in insertList)
            {
                var item = new CourseAssignedStudent
                {
                    CourseInstanceId = input.CourseInstanceId,
                    StudentId = studentId,
                    AssignedSource = AssignedSource.Direct,
                    Status = AssignedStatus.Invited
                };
                await _ws.InsertAsync<CourseAssignedStudent>(item);
            }

            //update
            var updateList = CASByCourses.Where(s => input.Students.Contains(s.StudentId));
            foreach (var student in updateList)
            {
                student.Status = AssignedStatus.Invited;
            }
            await _ws.UpdateRangeAsync(updateList);

            //delete
           /* await _ws.DeleteAsync<CourseAssignedStudent>(s => s.CourseInstanceId == input.CourseInstanceId && !input.Students.Contains(s.StudentId));*/

            await CurrentUnitOfWork.SaveChangesAsync();

            return insertList.ToList();
        }
        public async Task UnInviteToCourse(Guid CourseInstanceId,long StudentId)
        {
            var CASByCourses = _ws.GetAll<CourseAssignedStudent>().Where(cas => cas.CourseInstanceId == CourseInstanceId);
            var alreadyList = await CASByCourses.Select(s => s.StudentId).ToListAsync();
            if (!alreadyList.Contains(StudentId))
            {
                throw new UserFriendlyException("Người học chưa được mời vào khóa học");
            }
            await _ws.DeleteAsync<CourseAssignedStudent>(s => s.CourseInstanceId == CourseInstanceId && StudentId==s.StudentId);
        } 
        #endregion

        #region student enroll, accept, reject
        public async Task<CourseAssignedStudentDto> StudentEnrollCourse(StudentEnrollDto input)
        {
            var studentId = AbpSession.UserId.Value;
            var enrollmentSetting = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor).FirstOrDefaultAsync();
            bool isPending = enrollmentSetting != null && Boolean.Parse(enrollmentSetting.Value ?? "false");
            var isExist = await _ws.GetAll<CourseAssignedStudent>().AnyAsync(cas => cas.StudentId == studentId && cas.CourseInstanceId == input.CourseInstanceId);
            if (isExist) throw new UserFriendlyException(String.Format("Not valid"));
            var item = new CourseAssignedStudent
            {
                StudentId = studentId,
                CourseInstanceId = input.CourseInstanceId,
                AssignedSource = AssignedSource.FromEnroll,
                Status = isPending ? AssignedStatus.PendingApproved : AssignedStatus.Accepted,
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<CourseAssignedStudentDto>(item);
        }

        public async Task<CourseAssignedStudentDto> StudentReEnrollCourse(StudentEnrollDto input)
        {
            var studentId = AbpSession.UserId.Value;
            var enrollmentSetting = await _ws.GetAll<Setting, long>().Where(d => d.Name == AppSettingNames.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor).FirstOrDefaultAsync();
            bool isPending = enrollmentSetting != null && Boolean.Parse(enrollmentSetting.Value ?? "false");
            var course = await (from c in _ws.GetRepo<CourseInstance>().GetAllIncluding(s => s.Course)
                .Where(s => s.Id == input.CourseInstanceId)
                                join lms in _ws.GetAll<LMSSetting>().Where(s => s.EntityType == nameof(Course) && s.Name == Common.Const.Allow_completed_students_to_re_enroll)
                                on c.CourseId equals lms.EntityId into lmsSettings
                                from lmsSetting in lmsSettings.DefaultIfEmpty().Take(1)
                                select new { c.Course.Type, c.EndTime, AllowReEnroll = lmsSetting != null && Boolean.Parse(lmsSetting.Value) }).LastOrDefaultAsync();

            if (course.Type != CourseType.Recur)
            {
                throw new UserFriendlyException(String.Format("You can't re-enroll this course because the course type is not recur."));
            }
            if (!course.AllowReEnroll)
            {
                throw new UserFriendlyException(String.Format("You can't re-enroll this course because setting of course is not allow."));
            }
            var now = DateTime.UtcNow;
            if (course.EndTime != null && course.EndTime <= now)
            {
                throw new UserFriendlyException(String.Format("You can't re-enroll this course because the time was expire."));
            }

            var qalreadyAssingedStudent = _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == studentId && s.CourseInstanceId == input.CourseInstanceId);


            var assignedStudent = await qalreadyAssingedStudent.LastOrDefaultAsync();
            bool canReEnroll = (assignedStudent != null && assignedStudent.Status == AssignedStatus.Completed);

            if (!canReEnroll)
            {
                throw new UserFriendlyException(String.Format("student id {0} has not completed the course yet. So you can't re-enroll this course ", studentId));
            }

            var item = new CourseAssignedStudent
            {
                StudentId = studentId,
                CourseInstanceId = input.CourseInstanceId,
                AssignedSource = AssignedSource.FromEnroll,
                Status = isPending ? AssignedStatus.PendingApproved : AssignedStatus.Accepted,
                EnrollCount = await qalreadyAssingedStudent.CountAsync(s => s.Status == AssignedStatus.Completed)
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<CourseAssignedStudentDto>(item);
        }

        public async Task<CourseAssignedStudentDto> StudentAcceptRejectInvitaionCourse(StudentAcceptRejectDto input)
        {
            var studentId = AbpSession.UserId.Value;
            var isExist = await _ws.GetAll<CourseAssignedStudent>().AnyAsync(cas => cas.StudentId == studentId && cas.CourseInstanceId == input.CourseInstanceId);

            if (!isExist)
            {
                //not exist mean that student was invited by group -> insert a row into CourseAssignedStudent
                var item = new CourseAssignedStudent
                {
                    StudentId = studentId,
                    CourseInstanceId = input.CourseInstanceId,
                    AssignedSource = AssignedSource.Direct,
                    Status = input.Status
                };
                item.Id = await _ws.InsertAndGetIdAsync(item);
                return ObjectMapper.Map<CourseAssignedStudentDto>(item);
            }
            else
            {
                //invited by CourseAssignedStudent -> update
                var item = await _ws.GetAll<CourseAssignedStudent>().Where(cas => cas.StudentId == studentId && cas.CourseInstanceId == input.CourseInstanceId).FirstOrDefaultAsync();
                item.Status = input.Status;
                await _ws.GetRepo<CourseAssignedStudent>().UpdateAsync(item);
                return ObjectMapper.Map<CourseAssignedStudentDto>(item);
            }

        }
        #endregion
    }
}
