using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Entities
{
    public class LMSSetting : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid EntityId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string EntityType { get; set; }
        public string Value { get; set; }
    }
}
