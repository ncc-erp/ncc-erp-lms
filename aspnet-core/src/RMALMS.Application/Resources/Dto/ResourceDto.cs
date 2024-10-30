using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;

namespace RMALMS.Resources.Dto
{
    [AutoMapTo(typeof(Resource))]
    public class ResourceDto : EntityDto<Guid>
    {
        [ApplySearchAttribute]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string MineType { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }

    }
}
