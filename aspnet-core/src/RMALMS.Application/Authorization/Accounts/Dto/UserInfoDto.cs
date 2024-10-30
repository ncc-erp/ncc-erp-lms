using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Microsoft.AspNetCore.Http;
using RMALMS.UserStatus.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace RMALMS.Authorization.Accounts.Dto
{
    public class UserInfoDto : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<UserLinkDto> UserLinks { get; set; }
        public UserStatusDto Status { get; set; }
        public string Biography { get; set; }
        public virtual int? LanguageId { get; set; }
        public Guid? TimeZoneId { get; set; }
        public string StudentId { get; set; }
        public IEnumerable<AchievementDto> Archievements { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public IFormFile File { get; set; }
        public bool UserPersonalInfoViewByPublic { get; set; }
        public bool UserPersonalLinksViewByPublic { get; set; }
        public bool UserPersonalAchievementViewByPublic { get; set; }
        public bool UserPersonalCertificationViewByPublic { get; set; }
        public string BaseUtcOffset { get; set; }
    }

    public class AchievementDto
    {
        public string Level { get; set; }
        public int TotalCourse { get; set; }
    }
}
