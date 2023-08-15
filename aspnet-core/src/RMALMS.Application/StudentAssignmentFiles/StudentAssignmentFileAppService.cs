using Abp.Authorization;
using RMALMS.Entities;
using RMALMS.Roles.Dto;
using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Paging;
using RMALMS.IoC;
using Abp.Domain.Repositories;
using System.Linq;
using AutoMapper.QueryableExtensions;
using RMALMS.Extension;
using RMALMS.Authorization.Users;
using RMALMS.Resources.Dto;
using RMALMS.Helper;
using System.Collections.Generic;
using System.IO;
using RMALMS.StudentAssignmentFiles.Dto;
using Microsoft.EntityFrameworkCore;
using RMALMS.Courses.Dto;
using Abp.UI;
using RMALMS.DomainServices;

namespace RMALMS.StudentAssignmentFiles
{
    [AbpAuthorize]
    public class StudentAssignmentFileAppService : CrudApplicationBaseService<StudentAssignmentFile, StudentAssignmentFileDto, Guid, PagedResultRequestDto, CreateStudentAssignmentFileDto, StudentAssignmentFileDto>
    {
        private readonly IWorkScope _ws;
        private readonly IUploadHelper _uploadHelper;
        private readonly IStudentGroupManager _studentGroupManager;
        public StudentAssignmentFileAppService(
            IRepository<StudentAssignmentFile, Guid> respository,
            IWorkScope ws,
            IUploadHelper uploadHelper,
            IStudentGroupManager studentGroupManager
            )
            : base(respository)
        {
            _ws = ws;
            this._uploadHelper = uploadHelper;
            this._studentGroupManager = studentGroupManager;
        }
        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async override Task<StudentAssignmentFileDto> Create([FromForm] CreateStudentAssignmentFileDto input)
        {
            var user = _ws.GetRepo<User, long>().Get(AbpSession.UserId.Value);
            var assignObj = await _ws.GetRepo<AssignmentSetting>().GetAllIncluding(s => s.Assignment).Where(s => s.Id == input.AssignmentSettingId)
                .Select(s => new { Setting = s, s.Assignment }).LastOrDefaultAsync();


            var assignment = assignObj.Assignment;

            var courseAssignedStudent = await _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId && s.CourseInstanceId == assignObj.Setting.CourseInstanceId && s.Status == AssignedStatus.Accepted).LastOrDefaultAsync();
            if (courseAssignedStudent == null)
            {
                throw new UserFriendlyException(String.Format("The student id {0} is not accepted for this course", user.Id));
            }
            if (assignment.IsGroupAssignment && !assignment.IsAssignIndividualGrade)
            {
                var query = from gaa in _ws.GetAll<GroupAssingedAssignment>().Where(s => s.AssignmentSettingId == assignObj.Setting.Id)
                            join scg in _ws.GetAll<StudentCourseGroup>().Where( s => s.AssignedStudentId == courseAssignedStudent.Id)
                            on gaa.CourseGroupId equals scg.CourseGroupId                            
                            select gaa.CourseGroupId;
                input.CourseGroupId = await query.FirstOrDefaultAsync();
            }

            var coursegroup = await _ws.GetAll<CourseGroup>().Where(cs => cs.Id == input.CourseGroupId).FirstOrDefaultAsync();
            var course = await _ws.GetRepo<Course>().GetAsync(assignment.CourseId);
            string prefix = string.Empty;
            prefix = course.Name ?? "CourseName";
            prefix = string.Join("_", course.Name ?? "CourseName", assignment.IsGroupAssignment ? "assignment1" : "assignment2", coursegroup != null ? coursegroup.Name : user.FullName).Replace(" ", "");
            var item = ObjectMapper.Map<StudentAssignmentFile>(input);
            item.CourseAssignedStudentId = courseAssignedStudent.Id;
            //upload ImageCover 
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "StudentAssignmentFiles";
                var id = typeof(Guid).ChangeType(input.AssignmentSettingId) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";
                var file = await _uploadHelper.UploadFile(input.File, folder, prefix);
                item.FilePath = file.ServerPath;
                item.MineType = file.MineType;
            }
            item.Id = Guid.Empty;
            item.FileName = Path.GetFileName(item.FilePath);
            item.Id = await _ws.InsertAndGetIdAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<StudentAssignmentFileDto>(item);
        }
        public async override Task<StudentAssignmentFileDto> Update(StudentAssignmentFileDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }
        public async Task<List<StudentAssignmentFileDto>> GetStudentAssignmentFiles(Guid assignmentSettingId, Guid? courseAssignedStudentId)
        {            
            var userId = AbpSession.UserId.Value;
            var studentFiles = new List<StudentAssignmentFileDto>();

            var assignmentObj = await _ws.GetRepo<AssignmentSetting>()
                .GetAllIncluding(s => s.Assignment).Where(s => s.Id == assignmentSettingId)
                .Select(s => new { s.Assignment, s.CourseInstanceId }).LastOrDefaultAsync();
            var assignment = assignmentObj.Assignment;
            var courseInstanceId = assignmentObj.CourseInstanceId;
            if (!courseAssignedStudentId.HasValue)
            {
                var courseAssignedStudent = await _ws.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == userId).LastOrDefaultAsync();
                if (courseAssignedStudent == null || (courseAssignedStudent.Status != AssignedStatus.Accepted && courseAssignedStudent.Status != AssignedStatus.Completed))
                {
                    throw new UserFriendlyException(String.Format("User id {0} has not accepted for this course", userId));
                }
                courseAssignedStudentId = courseAssignedStudent.Id;
            }
            if (assignment.IsGroupAssignment && !assignment.IsAssignIndividualGrade)
            {
                var studentCourseGroupId = await this._studentGroupManager.GetCourseGroupdId(assignmentSettingId, courseAssignedStudentId.Value);

                studentFiles = await (from r in Repository.GetAll()
                                      where r.AssignmentSettingId == assignmentSettingId && r.CourseGroupId == studentCourseGroupId
                                      select new StudentAssignmentFileDto
                                      {
                                          Id = r.Id,
                                          FileName = r.FileName,
                                          FilePath = r.FilePath,
                                          MineType = r.MineType,
                                      }).ToListAsync();
            }
            else
            {
                studentFiles = await (from r in Repository.GetAll()
                                      where r.AssignmentSettingId == assignmentSettingId && r.CourseAssignedStudentId == courseAssignedStudentId
                                      select new StudentAssignmentFileDto
                                      {
                                          Id = r.Id,
                                          FileName = r.FileName,
                                          FilePath = r.FilePath,
                                          MineType = r.MineType,
                                      }).ToListAsync();
            }

            return studentFiles;
        }

        public async Task<StudentAssignmentFileDto> getFileByAssignmentSettingIdAndStudentId(Guid assignmentSettingId, Guid courseAssignedStudentId)
        {
            StudentAssignmentFileDto studentFile = null;
            var assignmentsetting = _ws.GetRepo<AssignmentSetting>().Get(assignmentSettingId);
            var assignment = _ws.GetRepo<Assignment>().Get(assignmentsetting.AssingmentId);

            if (assignment.IsGroupAssignment && !assignment.IsAssignIndividualGrade)
            {
                //assign to group

                var studentCourseGroupId = await this._studentGroupManager.GetCourseGroupdId(assignmentSettingId, courseAssignedStudentId);

                studentFile = await (from r in Repository.GetAll()
                                     where r.AssignmentSettingId == assignmentSettingId && r.CourseGroupId == studentCourseGroupId
                                     orderby r.CreationTime descending
                                     select new StudentAssignmentFileDto
                                     {
                                         Id = r.Id,
                                         FileName = r.FileName,
                                         FilePath = r.FilePath,
                                         MineType = r.MineType,
                                     }).FirstOrDefaultAsync();
            }
            else
            {
                //assign to each student
                studentFile = await (from r in Repository.GetAll()
                                     where r.AssignmentSettingId == assignmentSettingId && r.CourseAssignedStudentId == courseAssignedStudentId
                                     orderby r.CreationTime descending
                                     select new StudentAssignmentFileDto
                                     {
                                         Id = r.Id,
                                         FileName = r.FileName,
                                         FilePath = r.FilePath,
                                         MineType = r.MineType,
                                     }).FirstOrDefaultAsync();
            }

            return studentFile;
        }

        public override async Task Delete(EntityDto<Guid> input)
        {
            var resource = await Repository.GetAsync(input.Id);
            string postfix = string.Empty;
            string folder = string.Empty;
            folder = "StudentAssignmentFiles";
            var id = typeof(Guid).ChangeType(resource.AssignmentSettingId) as Guid?;
            if (id.HasValue)
            {
                postfix = id.Value.ToString();
            }
            postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
            folder = $"{folder}{postfix}";
            var filename = Path.GetFileName(resource.FilePath);
            _uploadHelper.DeleteFile(folder, filename);
            await Repository.DeleteAsync(input.Id);
        }
    }
}
