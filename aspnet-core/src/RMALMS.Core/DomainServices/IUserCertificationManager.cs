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
    public interface IUserCertificationManager : IDomainService
    {
        Task<UserCertification> CreateUpdateUserCertification(Guid courseAssignedStudentId, CertificationType certificationType, UpdateUserCertificationOption option );
        Task<UserCertification> CreateUpdateUserCertificationScorm(Guid courseAssignedStudentId, CertificationType certificationType, UpdateUserCertificationOption option);

    }
}
