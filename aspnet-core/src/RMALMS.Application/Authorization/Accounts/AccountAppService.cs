using Abp.Authorization;
using Abp.Configuration;
using Abp.UI;
using Abp.Zero.Configuration;
using CoreHtmlToImage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Accounts.Dto;
using RMALMS.Authorization.Users;
using RMALMS.Constants;
using RMALMS.DomainServices;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.Helper;
using RMALMS.Paging;
using RMALMS.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMALMS.Authorization.Accounts
{
    public class AccountAppService : RMALMSAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UploadHelper _uploadHelper;
        private readonly ICourseManager _courseManager;


        public AccountAppService(
            UserRegistrationManager userRegistrationManager, UploadHelper uploadHelper, ICourseManager courseManager)
        {
            _uploadHelper = uploadHelper;
            _userRegistrationManager = userRegistrationManager;
            this._courseManager = courseManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Account)]
        public async Task<UserInfoDto> GetUserProfile()
        {
            var quserCertifications =
                from cer in _ws.GetRepo<UserCertification>().GetAllIncluding(s => s.CourseAssignedStudent).Where(s => s.CourseAssignedStudent.StudentId == AbpSession.UserId)
                join ci in _ws.GetAll<CourseInstance>() on cer.CourseInstanceId equals ci.CourseId
                join c in _ws.GetAll<Course>() on ci.CourseId equals c.Id
                join cl in _ws.GetAll<CourseLevel>() on c.LevelId equals cl.Id
                select new { LevelId = cl.Id, LevelName = cl.DisplayName, CourseId = c.Id, UserId = cer.CourseAssignedStudent.StudentId };
            var ggroupUserCeritifications =
                from uc in quserCertifications
                group uc by new { uc.LevelId, uc.LevelName, uc.UserId } into g
                select new { g.Key.LevelName, g.Key.UserId, TotalCourse = g.Count() };

            var qusersIncludes = _ws.GetRepo<User, long>().GetAllIncluding(s => s.Status);

            var qusers =
                from u in qusersIncludes.Where(us => us.Id == AbpSession.UserId)
                join l in _ws.GetAll<UserLink>() on u.Id equals l.CreatorUserId into links
                join uc in ggroupUserCeritifications on u.Id equals uc.UserId into archs
                select new { User = u, Archievements = archs, UserLinks = links };

            var user = await qusers.FirstOrDefaultAsync();

            var userinfo = ObjectMapper.Map<UserInfoDto>(user.User);
            if (userinfo != null)
            {
                userinfo.Archievements = user.Archievements.Select(s => new AchievementDto { Level = s.LevelName, TotalCourse = s.TotalCourse });
                userinfo.UserLinks = ObjectMapper.Map<IEnumerable<UserLinkDto>>(user.UserLinks.ToList());
            }
            return userinfo;
        }

        private byte[] RenderImage(string htmlString)
        {
            if (htmlString.Contains("↵"))
            {
                htmlString = htmlString.Replace("↵", "");
            }
            var converter = new HtmlConverter();
            var bytes = converter.FromHtmlString(htmlString);
            //File.WriteAllBytes("Certificate.jpg", bytes);
            return bytes;
        }

        [AbpAuthorize(PermissionNames.Pages_AccountCeritification)]
        [HttpGet]
        public async Task<byte[]> PrintAsync(Guid courseCertificationId)
        {
            var courseCertification = (await GetAllCertificationsByUser(new GridParam { })).FirstOrDefault(s => s.Template.Id == courseCertificationId);
            if (courseCertification == default)
            {
                throw new UserFriendlyException(ResponseCode.NotFound.UserCourseCertification, "Not found course certification of user");
            }

            var htmlString = courseCertification.Template.Content;
            if (!htmlString.HasValue())
            {
                throw new UserFriendlyException(ResponseCode.NotFound.CertificationContent, "No content");
            }

            return RenderImage(htmlString);
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Account)]
        [HttpPut]
        public async Task<UserInfoDto> UpdateUserInfo(UserInfoDto userInfo)
        {
            var checkEmail = _ws.GetRepo<User, long>().GetAll().Any(x=>x.EmailAddress == userInfo.EmailAddress);
            if (checkEmail)
            {
                throw new UserFriendlyException(ResponseCode.NotFound.CertificationContent, "Email '"+userInfo.EmailAddress+"' is already taken.");

            }

            var user = _ws.GetRepo<User, long>().Get(AbpSession.UserId.Value);
            ObjectMapper.Map(userInfo, user);
            // update tenant id here
            user.Id = AbpSession.UserId.Value;

            var ulRepo = _ws.GetRepo<UserLink>();

            userInfo.UserLinks = userInfo.UserLinks ?? new List<UserLinkDto>();
            _ws.GetRepo<User, long>().Update(user);
            var existlinkIds = _ws.GetAll<UserLink>().Where(s => s.CreatorUserId == AbpSession.UserId.Value).Select(s => s.Id);
            userInfo.UserLinks = await _ws.InsertUpdateAndDelete<UserLink, UserLinkDto, Guid>(userInfo.UserLinks.ToList(), existlinkIds);
            if (user.TimeZoneId.HasValue)
            {
                userInfo.BaseUtcOffset = await UserManager.GetCurrentUtcOffsetAsync(user.TimeZoneId.Value);
            }
            return userInfo;
        }
        /// <summary>
        /// Upload images for Profile
        /// </summary>
        [HttpPost]
        public async Task<string> UploadImageFile([FromForm] IFormFile file)
        {
            var userFolder = $"Users/{AbpSession.UserId.Value}";

            var fileInfo = await _uploadHelper.UploadFile(file, userFolder);
            return fileInfo.ServerPath;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_AccountCeritification)]
        public async Task<List<UserCertificationDto>> GetAllCertificationsByUser(GridParam input)
        {
            var user = await _ws.GetRepo<User, long>().GetAsync(AbpSession.UserId.Value);
            var query = from ucs in _ws.GetRepo<UserCertification>().GetAllIncluding(s => s.CourseAssignedStudent, s => s.Template)
                        where ucs.CourseAssignedStudent.StudentId == user.Id
                        join ci in _ws.GetAll<CourseInstance>() on ucs.CourseInstanceId equals ci.Id
                        join cs in _ws.GetAll<Course>() on ci.CourseId equals cs.Id

                        select new UserCertificationDto
                        {
                            CourseInstanceId = ucs.CourseInstanceId,
                            Status = ucs.GraduatedLevel,
                            Certification = ucs.Certification,
                            CompletedDate = ucs.CreationTime,
                            Title = cs.Name,
                            UserName = user.FullName,
                            ImageCover = cs.ImageCover,
                            GraduatedLevel = ucs.GraduatedLevel,
                            Point = ucs.Point,
                            TotalPoint = ucs.TotalPoint,
                            Template = ucs.Template,
                        };
            var result = await query.Where(q => q.Template != null).ToListAsync();
            foreach (var item in result)
            {
                var completedTime = string.Format("{0} {1}, {2}", item.CompletedDate.ToString("MMM"), item.CompletedDate.ToString("dd"), item.CompletedDate.Year.ToString());
                var studentPercent = Math.Round(((item.Point / item.TotalPoint) * 100), 2);
                item.Template.Content = item.Template.Content
                                            .Replace("[_UserName_]", user.FullName)
                                            .Replace("[_CompletedDate_]", completedTime)
                                            .Replace("[_GradeSchemeLevel_]", item.GraduatedLevel)
                                            .Replace("[_TotalScore_]", item.TotalPoint.ToString())
                                            .Replace("[_StudentScore_]", item.Point.ToString())
                                            .Replace("[_StudentScorePercent_]", studentPercent.ToString());
            }
            return result;
        }
        public async Task<List<StudentScoreDto>> GetStudentsProgressInCourse(Guid courseInstanceId)
        {
            var user = _ws.GetRepo<User, long>().Get(AbpSession.UserId.Value);
            var query = _courseManager.GetAllStudentAssignedToCourse(courseInstanceId);
            var result = from u in query
                         join uc in _ws.GetRepo<UserCertification, Guid>().GetAllIncluding(uc => uc.CourseAssignedStudent).Where(uc => uc.CourseInstanceId == courseInstanceId)
                         on u.Id equals uc.CourseAssignedStudent.StudentId
                         into userScore
                         from uc in userScore.DefaultIfEmpty()
                         select new StudentScoreDto
                         {
                             StudentId = u.Id,
                             StudentName = u.FullName,
                             TotalScore = uc == null ? -1 : uc.TotalPoint,
                             StudentScore = uc == null ? -1 : uc.Point
                         };

            return await result.ToListAsync();
        }

    }
}
