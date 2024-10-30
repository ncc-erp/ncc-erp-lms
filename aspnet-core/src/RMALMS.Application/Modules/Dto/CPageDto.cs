using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Modules.Dto
{
    public class CPageDto: EntityDto<Guid>
    {
        public string Name { get; set; }
        public int SequenceOrder { get; set; }
        public string LinkType { get; set; }
        //public Guid LinkId { get; set; }


    }
}
