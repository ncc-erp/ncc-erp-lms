using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Pages.Dto
{
    public class PagesByCourseDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid? ModuleId { get; set; }
        public string ModuleName { get; set; }
    }
}
