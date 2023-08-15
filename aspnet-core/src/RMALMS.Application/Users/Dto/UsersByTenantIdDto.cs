using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Users.Dto
{
    public class UsersByTenantIdDto: GridParam
    {
        
        public int TenantId { get; set; }
        
    }
}
