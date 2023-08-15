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
    public interface ICourseManager : IDomainService
    {
        IQueryable<CourseAssignedStudent> GetStudentAssignedCourseByStatus(Guid courseInstanceId, AssignedStatus? Status, bool includingUser = true);
        IQueryable<CourseAssignedStudent> GetCourseAssignedStudentsAcceptedAndCompleted(Guid courseInstanceId);

        IQueryable<CourseGroup> GetCourseGroupsByCourse(Guid courseInstanceId, bool includeStudent = false);
        IQueryable<User> GetAllStudentAssignedToCourse(Guid courseInstanceId);

        Task CreateCourseInstance(Guid courseId);

        IQueryable<CourseAssignedStatus> GetAvailableCoursesForStudent(long userId);
        IQueryable<StudentAssignedQuizAssignment> GetStudentAssignedQuizzes(Guid courseInstanceId);
        Task<List<StudentQuizAssignmentScore>> GetStudentQuizScores(Guid assignedStudentId, Guid courseInstanceId);
        
        IQueryable<StudentAssignedQuizAssignment> GetStudentAssignedAssignments(Guid courseInstanceId);
        Task<List<StudentQuizAssignmentScore>> GetStudentAssignmentScores(Guid assignedStudentId, Guid courseInstanceId);

        Task<StudentScore> GetStudentScore(Guid assignedStudentId, Guid courseInstanceId);
        Task<StudentScore> GetStudentScoreScorm(Guid assignedStudentId, QuizScoreToKeepType? option);
        Task<List<StudentScormScore>> GetStudentQuizScoresScorm(Guid assignedStudentId, QuizScoreToKeepType? option);
        Task<CourseInstance> RePublishCourse(Guid courseInstanceId);
        Task CreateCourseDefaultTemplate(Guid courseId);
    }
}
