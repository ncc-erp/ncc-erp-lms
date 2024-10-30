using Abp.Domain.Entities;
using RMALMS.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class UserFollowQAQuestion : Entity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid FollowId { get; set; }
        public string FollowType { get; set; } // QAQuestion Or QAAnswer
    }
}
