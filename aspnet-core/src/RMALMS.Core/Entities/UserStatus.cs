using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public class UserStatus : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string Identifier { get; set; }
        public string DisplayName { get; set; } // it can be Basic, Proficient, Advanced
        public int? TenantId { get; set; }
        public int Level { get; set; }
        public bool IsStatic { get; set; } // could not delete Static status

        public CompareOperation? LowCompareOperation { get; set; }
        public int? RequiredNumber { get; set; }
    }
}
