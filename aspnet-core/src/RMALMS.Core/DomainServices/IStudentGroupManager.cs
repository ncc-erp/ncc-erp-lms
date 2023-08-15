using Abp.Domain.Services;
using RMALMS.Authorization.Users;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.DomainServices
{
    public interface IStudentGroupManager: IDomainService
    {        
        Task<Guid> GetCourseGroupdId(Guid assignmentSettingId, Guid courseAssignedStudentId);

    }
}
