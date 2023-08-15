using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using RMALMS.Anotations;
using RMALMS.Entities;

namespace RMALMS.Resources.Dto
{
    [AutoMapTo(typeof(Resource))]
    public class CreateResourceDto : EntityDto<Guid>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string MineType { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
        public IFormFile File { get; set; }

    }
}
