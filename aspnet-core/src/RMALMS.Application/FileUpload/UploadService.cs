using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMALMS.Extension;
using RMALMS.FileUpload.Dto;
using RMALMS.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.FileUpload
{
    public class UploadService : ApplicationService, IUploadService
    {
        readonly IUploadHelper _uploadHelper;
        readonly IConfiguration _configuration;
        public UploadService(
            IUploadHelper uploadHelper,
            IConfiguration configuration)
        {
            _uploadHelper = uploadHelper;
            _configuration = configuration;
        }
        public async Task<FileUploadInfo> UploadFile([FromForm] FileUploadDto file)
        {
            string postfix = string.Empty;
            string folder = string.Empty;
            switch (file.UploadType)
            {
                case UploadType.Course:
                    folder = "Course";
                    var id = typeof(Guid).ChangeType(file.Data) as Guid?;
                    if (id.HasValue)
                    {
                        postfix = id.Value.ToString();
                    }
                    break;
                default:
                    folder = "Course";
                    break;
            }
            postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
            folder = $"{folder}{postfix}";

            return await _uploadHelper.UploadFile(file.File, folder);
        }

        public async Task<ImgDto> UploadImg([FromForm] FileUploadDto file)
        {
            string postfix = string.Empty;
            string folder = string.Empty;
            switch (file.UploadType)
            {
                case UploadType.Course:
                    folder = "Course";
                    var id = typeof(Guid).ChangeType(file.Data) as Guid?;
                    if (id.HasValue)
                    {
                        postfix = id.Value.ToString();
                    }
                    break;
                default:
                    folder = "Course";
                    break;
            }
            postfix = postfix.Length > 0 ? $"-{postfix}" : postfix;
            folder = $"{folder}{postfix}";

            var fileInfo = await _uploadHelper.UploadFile(file.File, folder);
            return new ImgDto { Link = fileInfo.ServerPath };

        }
    }
}
