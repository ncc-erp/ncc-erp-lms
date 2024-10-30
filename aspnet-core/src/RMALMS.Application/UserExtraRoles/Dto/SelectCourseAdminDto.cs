using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserExtraRoles.Dto
{
    public class SelectCourseAdminDto
    {
        public string UserName  { get; set; }
        public long UserId { get; set; }
        public bool IsSelected { get; set; }
    }
}
