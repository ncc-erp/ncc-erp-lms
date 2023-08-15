using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Authorization.Accounts.Dto
{
    public class UserLinkDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Link { get; set; }
    }
}
