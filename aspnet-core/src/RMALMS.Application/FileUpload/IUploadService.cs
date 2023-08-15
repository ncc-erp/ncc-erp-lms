using RMALMS.FileUpload.Dto;
using RMALMS.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.FileUpload
{
    public interface IUploadService
    {
        Task<FileUploadInfo> UploadFile(FileUploadDto file);
    }
}
