using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Modules.Dto
{
    public class ModulesByCourseDto: EntityDto<Guid>
    {
        public string Name { get; set; }

    }
}
