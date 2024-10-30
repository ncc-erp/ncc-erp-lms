using Abp.Domain.Services;
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
    public interface ITestAttemptManager : IDomainService
    {
        Task<List<TestingTestAttempt>> GetAllTestingTestAttempt();
    }
}
