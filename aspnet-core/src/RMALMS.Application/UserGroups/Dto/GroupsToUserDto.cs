using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    public class GroupsToUserDto
    {
        public long UserId { get; set; }
        public Guid[] GroupIds { get; set; }
    }
}
