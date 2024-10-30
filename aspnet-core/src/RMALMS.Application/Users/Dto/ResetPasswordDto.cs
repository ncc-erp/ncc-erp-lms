using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Users.Dto
{
    public class ResetPasswordDto: Entity<long>
    {
        [Required]
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public int TenantId { get; set; }
    }
}
