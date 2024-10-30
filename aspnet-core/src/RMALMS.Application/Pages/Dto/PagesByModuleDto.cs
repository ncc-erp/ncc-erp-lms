using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Pages.Dto
{
    public class PagesByModuleDto: EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
