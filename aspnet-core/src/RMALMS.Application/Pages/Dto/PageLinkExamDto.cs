using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Pages.Dto
{
    [AutoMapTo(typeof(PageLinkExam))]
    public class PageLinkExamDto: EntityDto<Guid>
    {
        public Guid LinkId { get; set; }
        public string LinkType { get; set; }
        public int? SequenceOrder { get; set; }        
    }
}
