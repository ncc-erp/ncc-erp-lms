using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class UserExtraRole : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
    }    
}
