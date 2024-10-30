using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.TimeZone.Dto
{
    public class UserTimeZoneDto : EntityDto<Guid>
    {
        public string TimeZoneId { get; set; }
        public string DisplayName { get; set; }
    }
}
