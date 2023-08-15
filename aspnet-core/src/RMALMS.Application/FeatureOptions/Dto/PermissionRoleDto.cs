using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.FeatureOptions.Dto
{
    public class PermissionRoleDto
    {
        public long PermissonId { get; set; }
        public string PermissionName { get; set; }
        public bool IsGranted { get; set; }
        public int RoleId { get; set; }
    }
}
