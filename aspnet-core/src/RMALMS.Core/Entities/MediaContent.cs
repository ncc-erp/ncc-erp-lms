using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Entities
{
    public class MediaContent : Entity<Guid>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        public string Identifier { get; set; }
        public int? TenantId { get; set; }
    }
}
