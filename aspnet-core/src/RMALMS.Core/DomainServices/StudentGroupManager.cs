using Abp.Authorization.Users;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.DomainServices.Entity;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.DomainServices
{
    public class StudentGroupManager : BaseDomainService, IStudentGroupManager
    {
        public async Task<Guid> GetCourseGroupdId(Guid assignmentSettingId, Guid courseAssignedStudentId)
        {
            var _ws = WorkScope;
            var courseGroupId = await (from gaa in _ws.GetAll<GroupAssingedAssignment>()
                                        join scg in _ws.GetAll<StudentCourseGroup>()
                                        on gaa.CourseGroupId equals scg.CourseGroupId
                                        where gaa.AssignmentSettingId == assignmentSettingId && scg.AssignedStudentId == courseAssignedStudentId
                                        select gaa.CourseGroupId).FirstOrDefaultAsync();
            return courseGroupId;
        }
    }

}

