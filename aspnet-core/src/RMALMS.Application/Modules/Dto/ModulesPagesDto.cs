using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Modules.Dto
{
    public class ModulesPagesDto
    {
        public Guid CourseId { get; set; }
        public CModuleDto[] Modules { get; set; }
    }
}
