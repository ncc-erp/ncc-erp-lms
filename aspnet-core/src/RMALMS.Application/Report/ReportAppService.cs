using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Courses;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.IoC;
using RMALMS.Paging;
using RMALMS.Reports.Dto;
using RMALMS.StudentProgresses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RMALMS.Reports
{
    //[AbpAuthorize(Authorization.PermissionNames.Pages_Report)]
    [AbpAuthorize]
    public class ReportAppService : ApplicationService
    {
        private readonly IWorkScope _ws;

        public ReportAppService(IWorkScope workScope) : base()
        {
            this._ws = workScope;
        }

        /// <summary>
        /// Page admin/report - Using for Select Option
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<object>> GetsUserOption()
        {
            var qRole = from role in _ws.GetRepo<Role, int>().GetAll() // .Where(m => m.Name == StaticRoleNames.Tenants.Student)
                        join userRole in _ws.GetRepo<UserRole, long>().GetAll()
                        on role.Id equals userRole.RoleId
                        select new
                        {
                            UserId = userRole.UserId
                        };

            var qUser = from user in _ws.GetRepo<User, long>().GetAll().Where(m => m.IsActive == true)
                        join role in qRole.Distinct()
                        on user.Id equals role.UserId
                        select new
                        {
                            UserId = user.Id,
                            FullName = user.FullName,
                            UserName = user.UserName,
                        };
            var result = await qUser.OrderBy(m => m.UserId).ToListAsync();
            return new ListResultDto<object>(result);
        }

       

        /// <summary>
        /// Page admin/report - tab 0. User Login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupUserLoginInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qUserLog = from ulog in _ws.GetRepo<UserLoginAttempt, long>().GetAll()
                               //.Where(m => m.Result == AbpLoginResultType.Success)
                           select new ReportUserLoginDto
                           {
                               Id = ulog.Id,
                               IpAddress = ulog.ClientIpAddress, // == "::1" ? "127.0.0.1" : ulog.ClientIpAddress,
                               CreationTime = ulog.CreationTime,
                               UserId = ulog.UserId,
                               Action = ActionLogin(ulog.Result, ulog.TenancyName) // ulog.Result.ToString()
                           };

            qUserLog = SearchIncludeDate_Sort(input, qUserLog);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qUserLog.OrderByDescending(m => m.CreationTime)
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }


        /// <summary>
        /// Page admin/report - tab 1. User Activities
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupUserActivitiesInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qAuditLog = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                                .Where(m => m.ClientName.Contains(ActionReport.User_Activities.ToString()))
                            select new ReportUserLoginDto
                            {
                                Id = auditlog.Id,
                                IpAddress = auditlog.ClientIpAddress,
                                CreationTime = auditlog.ExecutionTime,
                                UserId = auditlog.UserId,
                                Action = auditlog.CustomData,
                            };
            // Search by date
            qAuditLog = SearchIncludeDate_Sort(input, qAuditLog);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qAuditLog
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }


        /// <summary>
        /// Page admin/report - tab 2. Student Statistics
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupStudentStatisticsInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qCourseVsInstance = GetsCourseJoinCouseInstance();

            var qAuditLog = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                                .Where(m => m.ClientName.Contains(ActionReport.Student_Statistics.ToString()))
                            from courseVsCourseIns in qCourseVsInstance
                            where (auditlog.Parameters.Contains(courseVsCourseIns.CourseId.ToString()) || auditlog.Parameters.Contains(courseVsCourseIns.CourseInstanceId.ToString()))
                            select new ReportUserLoginDto
                            {
                                Id = auditlog.Id,
                                IpAddress = auditlog.ClientIpAddress,
                                CreationTime = auditlog.ExecutionTime,
                                UserId = auditlog.UserId,
                                Action = auditlog.CustomData,
                                CourseName = courseVsCourseIns.CourseName,
                            };
            var qAuditLog_report_download = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                              .Where(m => m.ClientName.Contains(ActionReport.Student_Statistics_Course_Assignment_completed.ToString())
                                          )

                                            select new ReportUserLoginDto
                                            {
                                                Id = auditlog.Id,
                                                IpAddress = auditlog.ClientIpAddress,
                                                CreationTime = auditlog.ExecutionTime,
                                                UserId = auditlog.UserId,
                                                Action = auditlog.CustomData,
                                            };

            var qAuditLog_all = qAuditLog.Concat(qAuditLog_report_download);
            // Search by date
            qAuditLog = SearchIncludeDate_Sort(input, qAuditLog_all);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qAuditLog
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }

        /// <summary>
        /// Page admin/report - tab 3. Instructor Statistics
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupInstructorStatisticsInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qCourseVsInstance = GetsCourseJoinCouseInstance();

            var qAuditLog = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                                .Where(m => m.ClientName.Contains(ActionReport.Instructor_Statistics.ToString()))
                            from courseVsCourseIns in qCourseVsInstance
                            where (auditlog.Parameters.Contains(courseVsCourseIns.CourseId.ToString()) ||
                            auditlog.Parameters.Contains(courseVsCourseIns.CourseInstanceId.ToString()))
                            select new ReportUserLoginDto
                            {
                                Id = auditlog.Id,
                                IpAddress = auditlog.ClientIpAddress,
                                CreationTime = auditlog.ExecutionTime,
                                UserId = auditlog.UserId,
                                Action = auditlog.CustomData,
                                CourseName = courseVsCourseIns.CourseName,
                            };

            var qAuditLog_report_download = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                               .Where(m => m.ClientName.Contains(ActionReport.Instructor_Statistics_Report_download.ToString()) ||
                                            m.ClientName.Contains(ActionReport.Instructor_Statistics_Created_Course.ToString()))

                                            select new ReportUserLoginDto
                                            {
                                                Id = auditlog.Id,
                                                IpAddress = auditlog.ClientIpAddress,
                                                CreationTime = auditlog.ExecutionTime,
                                                UserId = auditlog.UserId,
                                                Action = auditlog.CustomData,
                                            };

            var qAuditLog_all = qAuditLog.Concat(qAuditLog_report_download);
            // Search by date
            qAuditLog = SearchIncludeDate_Sort(input, qAuditLog_all);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qAuditLog
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }


        /// <summary>
        /// Page admin/report - tab 4. Course Statistics
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupCourseStatisticsInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qCourseVsInstance = GetsCourseJoinCouseInstance();

            var qAuditLog = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                                .Where(m => m.ClientName.Contains(ActionReport.Course_Statistics.ToString()))
                            from courseVsCourseIns in qCourseVsInstance
                            where (auditlog.Parameters.Contains(courseVsCourseIns.CourseId.ToString()) || auditlog.Parameters.Contains(courseVsCourseIns.CourseInstanceId.ToString()))
                            select new ReportUserLoginDto
                            {
                                Id = auditlog.Id,
                                IpAddress = auditlog.ClientIpAddress,
                                CreationTime = auditlog.ExecutionTime,
                                UserId = auditlog.UserId,
                                Action = auditlog.CustomData,
                                CourseName = courseVsCourseIns.CourseName,
                            };
            var qAuditLog_report_download = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                               .Where(m => m.ClientName.Contains(ActionReport.Course_Statistics_Course_enrolled_and_invited_student.ToString()))

                                            select new ReportUserLoginDto
                                            {
                                                Id = auditlog.Id,
                                                IpAddress = auditlog.ClientIpAddress,
                                                CreationTime = auditlog.ExecutionTime,
                                                UserId = auditlog.UserId,
                                                Action = auditlog.CustomData,
                                            };
            var qAuditLog_all = qAuditLog.Concat(qAuditLog_report_download);
            // Search by date
            qAuditLog = SearchIncludeDate_Sort(input, qAuditLog_all);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qAuditLog
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }


        /// <summary>
        /// Page admin/report - tab 5. Course Import/Export
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GridResult<GroupUserLoginOut>> GetsGroupCourseImportExportInfo(GridParams input)
        {
            var qUser = GetsUser(input);

            var qCourseVsInstance = GetsCourseJoinCouseInstance();

            var qAuditLog = from auditlog in _ws.GetRepo<AuditLog, long>().GetAll()
                                .Where(m => m.ClientName.Contains(ActionReport.Course_Import_Export.ToString()))
                            from courseVsCourseIns in qCourseVsInstance
                            where (auditlog.Parameters.Contains(courseVsCourseIns.CourseId.ToString()) || auditlog.Parameters.Contains(courseVsCourseIns.CourseInstanceId.ToString()))
                            select new ReportUserLoginDto
                            {
                                Id = auditlog.Id,
                                IpAddress = auditlog.ClientIpAddress,
                                CreationTime = auditlog.ExecutionTime,
                                UserId = auditlog.UserId,
                                Action = auditlog.CustomData,
                                CourseName = courseVsCourseIns.CourseName,
                            };
            // Search by date
            qAuditLog = SearchIncludeDate_Sort(input, qAuditLog);

            var qUserGroupLogin = from user in qUser
                                  join ulog in qAuditLog
                                  on user.UserId equals ulog.UserId into ulogGroup
                                  select new GroupUserLoginOut
                                  {
                                      UserId = user.UserId,
                                      UserName = user.UserName,
                                      Users = ulogGroup,
                                      CountLogin = ulogGroup.Count(),
                                  };

            qUserGroupLogin = qUserGroupLogin.Where(m => m.CountLogin > 0);

            return await qUserGroupLogin.GetGridResult(qUserGroupLogin, input);
        }

        /// <summary>
        /// Log of Export action on page admin/Report
        /// </summary>
        /// <param name="input">action report name</param>
        /// <returns></returns>
        public async Task<bool> CreateExportOfReportAuditLog(ReportExportLogDto input)
        {
            AuditLog auditInfo = new AuditLog
            {
                //ClientName = ActionReport.User_Activities_Report_Downloaded.ToString(),
                ServiceName = "ReportAppService",
                CustomData = string.Format("Reports download: {0}", input.actionName),
                MethodName = "CreateExportOfReportAuditLog",
                ExecutionDuration = 100,
                ExecutionTime =Uitls.DateTimeUtils.GetNow(),
            };
            await _ws.GetRepo<AuditLog, long>().InsertAndGetIdAsync(auditInfo);
            return true;
        }








        #region Private
        private string ActionLogin(AbpLoginResultType result, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return string.Format("Login into {0} tenant", tenancyName);
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return "Failed to login";
                //case AbpLoginResultType.UserIsNotActive:
                //    break;
                //case AbpLoginResultType.InvalidTenancyName:
                //    break;
                //case AbpLoginResultType.TenantIsNotActive:
                //    break;
                //case AbpLoginResultType.UserEmailIsNotConfirmed:
                //    break;
                //case AbpLoginResultType.UnknownExternalLogin:
                //    break;
                case AbpLoginResultType.LockedOut:
                    return "Log off";
                //case AbpLoginResultType.UserPhoneNumberIsNotConfirmed:
                //    break;
                default:
                    return result.ToString();
            }
        }

        private string serviceName(ServiceName name)
        {
            return name.ToString().Replace('_', '.');
        }

        private IQueryable<GroupUserLoginOut> GetsUser(GridParams input)
        {
            var qUser = from user in _ws.GetRepo<User, long>().GetAll().Where(m => m.IsActive == true)
                        select new GroupUserLoginOut
                        {
                            UserId = user.Id,
                            UserName = user.FullName,
                        };
            if (input.UserId.HasValue)
            {
                qUser = qUser.Where(m => m.UserId == input.UserId);
            }
            if (qUser.Count() == 1) return qUser;
            if (!string.IsNullOrEmpty(input.SearchText))
            {
                var search = input.SearchText.Trim().ToLower();
                qUser = qUser.Where(m => m.UserName.ToLower().Contains(search) || m.UserId.ToString() == search);
            }
            return qUser;
        }

        private IQueryable<CourseId_CourseInstanceId_Name> GetsCourseJoinCouseInstance()
        {
            var qCourseVsInstance = from course in _ws.GetRepo<Course, Guid>().GetAll()
                                    join cInstan in _ws.GetRepo<CourseInstance, Guid>().GetAll().Where(m => m.Status == CourseSettingStatus.Active)
                                    on course.Id equals cInstan.CourseId
                                    select new CourseId_CourseInstanceId_Name
                                    {
                                        CourseId = course.Id,
                                        CourseInstanceId = cInstan.Id,
                                        CourseName = course.Name
                                    };
            return qCourseVsInstance;
        }

        private IQueryable<ReportUserLoginDto> SearchIncludeDate_Sort(GridParams input, IQueryable<ReportUserLoginDto> query)
        {
            if (input.FromDate.HasValue)
            {
                DateTime newFromDate = new DateTime(input.FromDate.Value.Year, input.FromDate.Value.Month, input.FromDate.Value.Day + 1); // date 1 => day = 0
                query = query.Where(m => newFromDate <= m.CreationTime);
            }
            if (input.ToDate.HasValue)
            {
                DateTime newToDate = new DateTime(input.ToDate.Value.Year, input.ToDate.Value.Month, input.ToDate.Value.Day + 2); // date 1 => day = 0
                query = query.Where(m => m.CreationTime <= newToDate);
            }
            if (!string.IsNullOrEmpty(input.SearchText))
            {
                var search = input.SearchText.Trim().ToLower();
                query = query.Where(m => (!string.IsNullOrEmpty(m.CourseName) && m.CourseName.ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(m.Action) && m.Action.ToLower().Contains(search)));

            }
            return query.OrderByDescending(m => m.CreationTime);
        }       

        #endregion Private
    }
}
