using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using RMALMS.Authorization.Accounts.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Users.Dto
{
    public class CourseInstructorDto : EntityDto<long>
    {
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<UserLinkDto> UserLinks { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime CreationTime { get; set; }
        public string[] RoleNames { get; set; }
        public Guid? StatusId { get; set; }
        public IEnumerable<AchievementDto> Achievement { get; set; }
    }
}
