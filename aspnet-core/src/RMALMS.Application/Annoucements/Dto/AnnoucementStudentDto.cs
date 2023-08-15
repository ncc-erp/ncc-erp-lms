using Abp.Application.Services.Dto;
using RMALMS.Anotations;
using System;

namespace RMALMS.Annoucements.Dto
{
    public class AnnoucementStudentDto: EntityDto<Guid>
    {

        [ApplySearchAttribute]
        public string Title { get; set; }
        public string Content { get; set; }
        //public Guid CourseInstanceId { get; set; }
        public string ImageCover { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreationTime { get; set; }
    }

}
