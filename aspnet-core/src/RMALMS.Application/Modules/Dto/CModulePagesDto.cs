using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Modules.Dto
{
    public class CModulePagesDto : EntityDto<Guid>
    {
        public Guid ModuleId { get; set; }
        public CPageDto[] Pages { get; set; }
    }
}
