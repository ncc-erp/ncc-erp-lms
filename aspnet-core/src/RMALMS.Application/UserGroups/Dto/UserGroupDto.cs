using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    [AutoMapTo(typeof(Entities.UserGroup))]
    public class UserGroupDto: EntityDto<Guid>
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string FullName { get; set; }
        public string ImageCover { get; set; }
        public string Email { get; set; }
    }
}
