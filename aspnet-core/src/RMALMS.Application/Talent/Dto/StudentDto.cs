using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Talent.Dto
{
    [AutoMapTo(typeof(User))]
    public class StudentTalentDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Guid CourseInstanceId { get; set; }
    }
}
