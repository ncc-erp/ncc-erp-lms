using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Localization;
using RMALMS.Entities;

namespace RMALMS.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();
            return user;
        }

        [ForeignKey(nameof(StatusId))]
        public UserStatus Status { get; set; }
        public Guid? StatusId { get; set; }
        public string Biography { get; set; }
        public string DisplayName { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public virtual ApplicationLanguage Language { get; set; }
        public virtual int? LanguageId { get; set; }
        [ForeignKey(nameof(TimeZoneId))]
        public UserTimeZone TimeZone { get; set; }
        public Guid? TimeZoneId { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public bool UserPersonalInfoViewByPublic { get; set; }
        public bool UserPersonalLinksViewByPublic { get; set; }
        public bool UserPersonalAchievementViewByPublic { get; set; }
        public bool UserPersonalCertificationViewByPublic { get; set; }
        //public List<UserLink> UserLinks { get; set; }
    }
}
