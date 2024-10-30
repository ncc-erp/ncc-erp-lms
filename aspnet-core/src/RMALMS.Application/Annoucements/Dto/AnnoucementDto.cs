using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Authorization.Roles;
using RMALMS.Entities;

namespace RMALMS.Annoucements.Dto
{
    [AutoMapTo(typeof(Annoucement))]
    public class AnnoucementDto : EntityDto<Guid>
    {
        [ApplySearchAttribute]
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CourseInstanceId { get; set; }
        public string ImageCover { get; set; }
        public string UserName { get; set; }
        public  DateTime CreationTime { get; set; }


    }
}
