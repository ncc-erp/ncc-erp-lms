using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Modules.Dto
{
    public class CModuleDto: EntityDto<Guid>
    {
        
        public string Name { get; set; }
        public int SequenceOrder { get; set; }
        public IEnumerable<CPageDto> Pages { get; set; }
    }
}
