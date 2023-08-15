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
    public interface IQuizManager : IDomainService
    {
        Task<TestAttempt> ProcessScore(long studentId, Guid testAttemptId, PageType questionType);
        Task CompletedCourse(Guid courseAssignedStudentId);
    }
}
