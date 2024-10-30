using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    public class UsersByGroupDto: Entity<long>
    {
        public string Name { get; set; }
    }
}
