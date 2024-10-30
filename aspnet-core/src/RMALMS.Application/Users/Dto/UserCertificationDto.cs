using RMALMS.Entities;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Users.Dto
{
    public class UserCertificationDto : GridParam
    {

        public long StudentId { get; set; }
        public Guid CourseInstanceId { get; set; }
        public string Status { get; set; }
        public string Certification { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string ImageCover { get; set; }
        public Guid CourseId { get; set; }
        public CourseCertificationTemplate Template { get; set; }
        public string GraduatedLevel { get; set; }
        public float Point { get; set; }
        public float TotalPoint { get; set; }

    }
}
