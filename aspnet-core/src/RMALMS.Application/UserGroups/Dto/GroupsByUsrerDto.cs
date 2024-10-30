using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserGroups.Dto
{
    public class GroupsByUsrerDto: EntityDto<Guid>
    {        
        public string Name { get; set; }
    }
}
