using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMALMS.CertificationTemplates.Dto;
using RMALMS.Entities;
using RMALMS.Extension;
using RMALMS.Helper;
using RMALMS.IoC;
using RMALMS.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.CertificationTemplates
{
    [AbpAuthorize]
    public class CertificationTemplateAppService : CrudApplicationBaseService<CourseCertificationTemplate, CertificationTemplateDto, Guid, PagedResultRequestDto, CertificationTemplateDto, CertificationTemplateDto>
    {
        private readonly IWorkScope _ws;
        private readonly IUploadHelper _uploadHelper;

        public CertificationTemplateAppService(
            IRepository<CourseCertificationTemplate,
                Guid> respository,
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

        public async Task<List<CertificationTemplateDto>> getByCourseId(Guid CourseId)
        {
            var query = Repository.GetAll().Where(r => r.CourseId == CourseId).ProjectTo<CertificationTemplateDto>();
            return await query.ToListAsync();
        }

        public async override Task<CertificationTemplateDto> Create([FromForm] CertificationTemplateDto input)
        {
            CheckCreatePermission();
            var item = ObjectMapper.Map<CourseCertificationTemplate>(input);

            item.Id = await _ws.InsertAndGetIdAsync(item);
            await CurrentUnitOfWork.SaveChangesAsync();
            //upload ImageCover 
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "Certification";
                var id = typeof(Guid).ChangeType(item.Id) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var img = await _uploadHelper.UploadFile(input.File, folder);
                item.Background = img.ServerPath;

                await _ws.GetRepo<CourseCertificationTemplate>().UpdateAsync(item);
            }
            return ObjectMapper.Map<CertificationTemplateDto>(item);
        }
        public async override Task<CertificationTemplateDto> Update([FromForm] CertificationTemplateDto input)
        {
            CheckUpdatePermission();
            var item = await Repository.GetAsync(input.Id);
            if (input.File != null)
            {
                string postfix = string.Empty;
                string folder = string.Empty;
                folder = "Certification";
                var id = typeof(Guid).ChangeType(item.Id) as Guid?;
                if (id.HasValue)
                {
                    postfix = id.Value.ToString();
                }
                postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
                folder = $"{folder}{postfix}";

                var img = await _uploadHelper.UploadFile(input.File, folder);
                input.Background = img.ServerPath;
            }
            else
            {
                input.Background = item.Background;
            }
            ObjectMapper.Map(input, item);
            await _ws.UpdateAsync(item);

            return input;
        }
    }
}
