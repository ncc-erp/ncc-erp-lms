using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Newtonsoft.Json;
using RMALMS.Anotations;
using RMALMS.Authorization.Users;

namespace RMALMS.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto :  EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        [ApplySearchAttribute]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        [ApplySearchAttribute]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        [ApplySearchAttribute]
        public string FullName { get; set; }

        public string Title { get; set; }

        public string DisplayName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }
        public Guid? StatusId { get; set; }

        //public UserCreaterDto Creater { get; set; }
        [ApplySearchAttribute]
        public string CreatorName { get; set; }
        public bool IsCreatedByRoot { get; set; }
    }

    [AutoMapFrom(typeof(User))]
    public class SelectableStudentDto
    {
        [JsonIgnore]
        [ApplySearch]
        public string DisplayName { get; set; }
        public long Id { get; set; }
        [JsonIgnore]
        [ApplySearch]
        public string FirstName { get; set; }
        [JsonIgnore]
        [ApplySearch]
        public string LastName { get; set; }
        public string Name { get { return DisplayName ?? $"{FirstName} {LastName}"; } }
    }

    public class UserCreaterDto
    {
        public string UserName { get; set; }        
        public string Name { get; set; }      
        public string Surname { get; set; }       
        public string EmailAddress { get; set; }
    }

}
