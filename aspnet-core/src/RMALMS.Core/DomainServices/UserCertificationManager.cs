using Abp.Authorization.Users;
using Abp.Domain.Services;
using Abp.UI;
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
    public class UserCertificationManager : BaseDomainService, IUserCertificationManager
    {
        private ICourseManager _courseManager;
        public UserCertificationManager(ICourseManager courseManager) : base()
        {
            this._courseManager = courseManager;
        }
        public async Task<UserCertification> CreateUpdateUserCertification(Guid courseAssignedStudentId, CertificationType type, UpdateUserCertificationOption option)
        {
            var _ws = WorkScope;
            var courseInstance = await _ws.GetRepo<CourseAssignedStudent>().GetAllIncluding(s => s.CourseInstance)
                .Where(s => s.Id == courseAssignedStudentId).Select(s => s.CourseInstance).LastOrDefaultAsync();
            if (courseInstance == null)
            {
                throw new UserFriendlyException("Null CourseAssignedStudent or CourseInstance");
            }
            var courseId = courseInstance.CourseId;
            var courseInstanceId = courseInstance.Id;

            var template = (await _ws.GetAll<CourseCertificationTemplate>()
                .Where(s => s.CourseId == courseId && s.IsActive && s.CertificationType == type)
                .FirstOrDefaultAsync());

            var qcertification = _ws.GetAll<UserCertification>()
                .Where(s => s.CourseAssignedStudentId == courseAssignedStudentId);
            UserCertification cer = null;
            if (template != null)
                cer = await qcertification.Where(s => s.TemplateId == template.Id).FirstOrDefaultAsync();
            else
                cer = await qcertification.Where(s => !s.TemplateId.HasValue).FirstOrDefaultAsync();

            if (option == UpdateUserCertificationOption.UpdateOnly && cer == null)
            {
                if (template != null)
                {
                    cer = await qcertification.Where(s => !s.TemplateId.HasValue).FirstOrDefaultAsync();
                    if (cer != null)
                    {
                        cer.TemplateId = template.Id;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            //score
            StudentScore studentScore = null;
            string gradeLevel = null;
            if (type == CertificationType.Completion)
            {
                studentScore = await _courseManager.GetStudentScore(courseAssignedStudentId, courseInstanceId);
                var point = studentScore.StudentPoint * 100 / studentScore.CoursePoint;
                //apply grade            
                if (courseInstance.EnableCourseGradingScheme)
                {
                    gradeLevel = await applyGrade(courseId, point);
                }
            }


            if (cer == null)
            {

                cer = new UserCertification
                {
                    CourseAssignedStudentId = courseAssignedStudentId,
                    CourseInstanceId = courseInstanceId,
                    TemplateId = template == null ? (Guid?)null : template.Id
                };
                if (type == CertificationType.Completion)
                {
                    cer.Point = studentScore != null ? studentScore.StudentPoint : 0;
                    cer.TotalPoint = studentScore != null ? studentScore.CoursePoint : 0;
                    cer.GraduatedLevel = gradeLevel;
                }
                cer.Id = await _ws.InsertAndGetIdAsync(cer);
            }
            else
            {
                if (type == CertificationType.Completion && cer.Point != studentScore.StudentPoint)
                {
                    cer.Point = studentScore.StudentPoint;
                    cer.TotalPoint = studentScore.CoursePoint;
                    cer.GraduatedLevel = gradeLevel;
                    await _ws.UpdateAsync(cer);
                }
                if (cer.TemplateId == null && template != null)
                {
                    cer.TemplateId = template.Id;
                    await _ws.UpdateAsync(cer);
                }
            }

            return cer;
        }

        public async Task<UserCertification> CreateUpdateUserCertificationScorm(Guid courseAssignedStudentId, CertificationType type, UpdateUserCertificationOption option)
        {
            var _ws = WorkScope;
            var courseInstance = await _ws.GetRepo<CourseAssignedStudent>().GetAllIncluding(s => s.CourseInstance)
                .Where(s => s.Id == courseAssignedStudentId).Select(s => s.CourseInstance).LastOrDefaultAsync();
            if (courseInstance == null)
            {
                throw new UserFriendlyException("Null CourseAssignedStudent or CourseInstance");
            }
            var courseId = courseInstance.CourseId;
            var courseInstanceId = courseInstance.Id;

            var template = (await _ws.GetAll<CourseCertificationTemplate>()
                .Where(s => s.CourseId == courseId && s.IsActive && s.CertificationType == type)
                .FirstOrDefaultAsync());

            var qcertification = _ws.GetAll<UserCertification>()
                .Where(s => s.CourseAssignedStudentId == courseAssignedStudentId);
            UserCertification cer = null;
            if (template != null)
                cer = await qcertification.Where(s => s.TemplateId == template.Id).FirstOrDefaultAsync();
            else
                cer = await qcertification.Where(s => !s.TemplateId.HasValue).FirstOrDefaultAsync();

            if (option == UpdateUserCertificationOption.UpdateOnly && cer == null)
            {
                if (template != null)
                {
                    cer = await qcertification.Where(s => !s.TemplateId.HasValue).FirstOrDefaultAsync();
                    if (cer != null)
                    {
                        cer.TemplateId = template.Id;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            //score
            StudentScore studentScore = null;
            string gradeLevel = null;
            if (type == CertificationType.Completion)
            {
                studentScore = await _courseManager.GetStudentScoreScorm(courseAssignedStudentId, QuizScoreToKeepType.Highest);

                var point = studentScore.StudentPoint * 100 / studentScore.CoursePoint;

                //apply grade            
                if (courseInstance.EnableCourseGradingScheme)
                {
                    gradeLevel = await applyGrade(courseId, point);                    
                }
            }


            if (cer == null)
            {

                cer = new UserCertification
                {
                    CourseAssignedStudentId = courseAssignedStudentId,
                    CourseInstanceId = courseInstanceId,
                    TemplateId = template == null ? (Guid?)null : template.Id
                };
                if (type == CertificationType.Completion)
                {
                    cer.Point = studentScore != null ? studentScore.StudentPoint : 0;
                    cer.TotalPoint = studentScore != null ? studentScore.CoursePoint : 0;
                    cer.GraduatedLevel = gradeLevel;
                }
                cer.Id = await _ws.InsertAndGetIdAsync(cer);
            }
            else
            {
                if (type == CertificationType.Completion && cer.Point != studentScore.StudentPoint)
                {
                    cer.Point = studentScore.StudentPoint;
                    cer.TotalPoint = studentScore.CoursePoint;
                    cer.GraduatedLevel = gradeLevel;
                    await _ws.UpdateAsync(cer);
                }
                if (cer.TemplateId == null && template != null)
                {
                    cer.TemplateId = template.Id;
                    await _ws.UpdateAsync(cer);
                }
            }

            return cer;
        }

        private async Task<string> applyGrade(Guid courseId, float point)
        {
            
            var _ws = WorkScope;
            var grade = await(from g in _ws.GetAll<GradeScheme>().Where(s => s.CourseId == courseId && s.Status == GradeSchemeStatus.Active)
                              join e in _ws.GetAll<GradeSchemeElement>()
                              on g.Id equals e.GradeSchemeId into list
                              where list.Any()
                              select new
                              {
                                  GradeName = g.Title,
                                  Elements = list.OrderByDescending(s => s.HighRange).Select(s => new { s.Name, s.LowRange, s.LowCompareOperation, s.HighRange, s.HighCompareOpertion })
                                  //sort big -> small
                              }).FirstOrDefaultAsync();
            if (grade != null)
            {
                foreach (var e in grade.Elements)
                {
                    if (e.LowCompareOperation == CompareOperation.GreaterThan) //>
                    {
                        if (point > e.LowRange)
                        {
                            return e.Name;                            
                        }
                    }
                    else //>=
                    {
                        if (point >= e.LowRange)
                        {
                            return e.Name;                            
                        }
                    }
                }
            }
            return "";
        }
    }

    public enum UpdateUserCertificationOption
    {
        UpdateOnly = 0,
        UpdateIfNotExistInsert = 1
    }
}

