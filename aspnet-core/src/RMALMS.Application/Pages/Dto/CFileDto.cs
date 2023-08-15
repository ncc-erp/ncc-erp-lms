using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Pages.Dto
{
    public class CFileDto: EntityDto<Guid>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
