using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    public class GroupCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class GroupUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class GroupIncludeUserDto : GroupUpdateDto
    {
        public IEnumerable<UserGroupDto> UserGroups { get; set; }
    }

    public class UserStudentDto
    {
        public long? UserId { get; set; }
        public string ImageCover { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int CountGroup { get; set; }

    }

    public class UserGroupInput
    {
        public long UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid? GroupId_old { get; set; }

    }

}
