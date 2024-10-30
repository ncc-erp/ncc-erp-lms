using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using RMALMS.Anotations;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RMALMS.CertificationTemplates.Dto
{
    [AutoMapTo(typeof(CourseCertificationTemplate))]
    public class CertificationTemplateDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public bool IsActive { get; set; }
        public string Background { get; set; }
        public string Content { get; set; }
        public CertificationType CertificationType { get; set; }
        public IFormFile File { get; set; }
        public CertificationOrientation Orientation { get; set; }


    }
}
