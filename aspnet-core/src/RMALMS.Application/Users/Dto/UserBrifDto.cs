using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Users.Dto
{
    /// <summary>
    /// chose user from list user to add to group
    /// </summary>
    public class UserBrifDto: EntityDto<long>
    {        
        public string UserName { get; set; }        
        public bool IsActive { get; set; }
        public string StatusName { get; set; }
    }
}
