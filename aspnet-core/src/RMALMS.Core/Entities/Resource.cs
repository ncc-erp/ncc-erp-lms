using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Entities
{
    public class Resource : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        public string MineType { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
    }
}
