using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public class UserTimeZone : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string TimeZoneId { get; set; }
        public int? TenantId { get; set; }
        public string DisplayName { get; set; }
        public bool SupportsDaylightSavingTime { get; set; }
        public string BaseUtcOffset { get; set; }
        public string StandardName { get; set; }
    }
}
