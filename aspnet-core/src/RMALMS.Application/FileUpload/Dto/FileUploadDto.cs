using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.FileUpload.Dto
{
    public class FileUploadDto
    {
        public IFormFile File { get; set; }
        public string Data { get; set; }
        public UploadType UploadType { get; set; }
    }

    public enum UploadType
    {
        Course = 0
    }
}
