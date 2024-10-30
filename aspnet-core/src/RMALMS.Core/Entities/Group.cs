using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Group : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(ParentId))]
        public Group Parent { get; set; }
        public Guid? ParentId { get; set; }
    }
}
