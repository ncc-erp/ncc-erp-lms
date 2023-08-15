using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class UserGroup : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }
        public Guid GroupId { get; set; }
    }
}
