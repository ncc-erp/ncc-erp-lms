using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    public class UsersToGroupDto
    {
        public Guid GroupId { get; set; }
        public long[] UserIds { get; set; }
    }
}
