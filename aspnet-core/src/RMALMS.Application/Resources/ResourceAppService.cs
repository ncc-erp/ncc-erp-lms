using Abp.Authorization;
using RMALMS.Entities;
using RMALMS.Roles.Dto;
using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Paging;
using RMALMS.IoC;
using Abp.Domain.Repositories;
using System.Linq;
using AutoMapper.QueryableExtensions;
using RMALMS.Extension;
using RMALMS.Authorization.Users;
using RMALMS.Resources.Dto;
using RMALMS.Helper;
using System.Collections.Generic;
using System.IO;

namespace RMALMS.Resources
{
    [AbpAuthorize]
    public class ResourceAppService : CrudApplicationBaseService<Resource, ResourceDto, Guid, PagedResultRequestDto, CreateResourceDto, ResourceDto>
    {
        private readonly IWorkScope _ws;
        private readonly IUploadHelper _uploadHelper;
        public ResourceAppService(
            IRepository<Resource, Guid> respository,
            IWorkScope ws,
            IUploadHelper uploadHelper
            )
            : base(respository)
        {
            _ws = ws;
            this._uploadHelper = uploadHelper;
        }
        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async override Task<ResourceDto> Create([FromForm] CreateResourceDto input)
        {
            var item = ObjectMapper.Map<Resource>(input);
            //upload ImageCover 
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = input.EntityType;
                var id = typeof(Guid).ChangeType(input.EntityId) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var file = await _uploadHelper.UploadFile(input.File, folder);
                item.FilePath = file.ServerPath;
            }
            item.Id = Guid.Empty;
            if (string.IsNullOrEmpty(input.FileName))
                item.FileName = Path.GetFileName(item.FilePath);
            item.Id = await _ws.InsertAndGetIdAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<ResourceDto>(item);
        }
        public async override Task<ResourceDto> Update(ResourceDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }
        public async Task<List<ResourceDto>> getByEntityId(Guid EntityId, string EntityType)
        {

            var queryresult = from r in Repository.GetAll()
                              where r.EntityId == EntityId && r.EntityType == EntityType
                              select new ResourceDto
                              {
                                  Id = r.Id,
                                  FileName = r.FileName,
                                  FilePath = r.FilePath,
                                  MineType = r.MineType,
                                  EntityId = r.EntityId,
                                  EntityType = r.EntityType
                              };
            return queryresult.ToList();
        }

        public override async Task Delete(EntityDto<Guid> input)
        {
            var resource = await Repository.GetAsync(input.Id);
            string postfix = string.Empty;
            string folder = string.Empty;
            folder = resource.EntityType;
            var id = typeof(Guid).ChangeType(resource.EntityId) as Guid?;
            if (id.HasValue)
            {
                postfix = id.Value.ToString();
            }
            postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
            folder = $"{folder}{postfix}";
            var filename = Path.GetFileName(resource.FilePath);
            _uploadHelper.DeleteFile(folder, filename);
            await Repository.DeleteAsync(input.Id);
        }
    }
}
