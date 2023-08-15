using AutoMapper;
using RMALMS.Entities;
using RMALMS.StudentProgresses.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Abp.Authorization;
using Abp.UI;
using RMALMS.DomainServices;
using Microsoft.AspNetCore.Mvc;

namespace RMALMS.StudentProgresses
{
    [AbpAuthorize]
    public class StudentProgressAppService : ApplicationBaseService
    {
        private readonly IUserCertificationManager _userCertificationManager;

        public StudentProgressAppService(
            IUserCertificationManager userCerfiticationManager
            ) : base()
        {
            this._userCertificationManager = userCerfiticationManager;
        }

        public async Task<StudentProgressDto> Create(StudentProgressDto input)
        {
            var courseAssignedStudentId = (await _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId.Value && s.CourseInstanceId == input.CourseInstanceId).LastOrDefaultAsync()).Id;
            var isExist = await _ws.GetAll<StudentProgress>().AnyAsync(sp => sp.CourseInstanceId == input.CourseInstanceId && sp.PageId == input.PageId && sp.CourseAssignedStudentId == courseAssignedStudentId);
            if (!isExist)
            {
                var item = ObjectMapper.Map<StudentProgress>(input);
                item.Progress = StudentProgressStatus.Studying;
                item.CourseAssignedStudentId = courseAssignedStudentId;

                input.Id = await _ws.InsertAndGetIdAsync(item);
                return input;
            }
            else
            {
                return null;
            }

        }

        public async Task<StudentProgressDto> Update(StudentProgressDto input)
        {
            var item = await _ws.GetRepo<StudentProgress>().GetAsync(input.Id);
            ObjectMapper.Map<StudentProgressDto, StudentProgress>(input, item);
            await _ws.UpdateAsync(item);
            return ObjectMapper.Map<StudentProgressDto>(item);

        }

        public async Task<List<StudentProgressDto>> GetStudentProgressesByCourseInstanceId(Guid courseInstanceId)
        {
            var courseAssignedStudent = await _ws.GetAll<CourseAssignedStudent>().Where(s => s.StudentId == AbpSession.UserId.Value && s.CourseInstanceId == courseInstanceId).LastOrDefaultAsync();
            if (courseAssignedStudent == null || (courseAssignedStudent.Status != AssignedStatus.Accepted && courseAssignedStudent.Status != AssignedStatus.Completed))
            {
                throw new UserFriendlyException("You don't have permission to view this course");
            }
            var query = _ws.GetAll<StudentProgress>().Where(sp => sp.CourseInstanceId == courseInstanceId && sp.CourseAssignedStudentId == courseAssignedStudent.Id).ProjectTo<StudentProgressDto>();
            return await query.ToListAsync();
        }
        [HttpGet]
        public async Task<UserCertification> CreateUserAttendanceCertification(Guid courseInstanceId)
        {
            var userId = AbpSession.UserId.Value;
            var courseAssignedStudent = await _ws.GetAll<CourseAssignedStudent>().Where(s => s.CourseInstanceId == courseInstanceId && s.StudentId == userId).LastOrDefaultAsync();
            if (courseAssignedStudent != null && (courseAssignedStudent.Status == AssignedStatus.Accepted || courseAssignedStudent.Status == AssignedStatus.Completed))
            {
                await _userCertificationManager.CreateUpdateUserCertification(courseAssignedStudent.Id, CertificationType.Attendance, UpdateUserCertificationOption.UpdateIfNotExistInsert);
            }
            return null;
        }
    }
}
