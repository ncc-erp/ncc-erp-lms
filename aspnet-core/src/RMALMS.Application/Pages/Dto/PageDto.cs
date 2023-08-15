using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Pages.Dto
{
    //[AutoMapTo(typeof(Page))]
    public class PageDto:EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string Content { get; set; }
        public Guid ModuleId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        public int SequenceOrder { get; set; }
        public PageType Type { get; set; }
        public List<PageLinkExamDto> Links { get; set; }
        public bool Bookmarked { get; set; }
        public IEnumerable<CFileDto> Files { get; set; }
    }
}
