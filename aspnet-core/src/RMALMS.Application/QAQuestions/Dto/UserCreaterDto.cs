using Abp.Application.Services.Dto;
using RMALMS.Paging;
using System;

namespace RMALMS.QAQuestions.Dto
{
    public class UserCreaterDto: EntityDto<Guid>
    {
        public long? UserId { get; set; }
        public string ImageCover { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsFollow { get; set; }
    }

}
