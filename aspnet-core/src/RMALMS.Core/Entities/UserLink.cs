using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public class UserLink : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public int? TenantId { get; set; }
    }
}
