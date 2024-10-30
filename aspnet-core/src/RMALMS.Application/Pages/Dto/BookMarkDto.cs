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
    public class BookMarkOut : EntityDto<Guid>
    {
        public string ModuleName { get; set; }
        public Guid ModuleId { get; set; }
        public string PageName { get; set; }
        public Guid PageId { get; set; }
        public DateTime CreationTime { get; set; }
    }
    public class BookMarkInputDto: EntityDto<Guid>
    {       
        public Guid CourseInstanceId { get; set; }      
        public Guid PageId { get; set; }        
    }

}
