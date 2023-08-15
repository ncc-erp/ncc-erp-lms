using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Entities;
using System.ComponentModel.DataAnnotations;

namespace RMALMS.UserGroups.Dto
{
    [AutoMapTo(typeof(RMALMS.Entities.UserGroup))]
    public class CreateUserGroupDto
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public Guid GroupId { get; set; }
    }
}
