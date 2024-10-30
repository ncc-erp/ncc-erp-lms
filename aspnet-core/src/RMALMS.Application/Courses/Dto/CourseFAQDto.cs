using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RMALMS.Anotations;

namespace RMALMS.Courses.Dto
{
    public class CourseFAQDto
    {
        public Guid Id { get; set; }
        public Guid CourseInstanceId { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalFAQ { get; set; }
        public int TotalQuestion { get; set; }
        public int TotalResponse { get; set; }
        public bool IsReadedQuestion { get; set; }
        public bool IsReadedResponse { get; set; }
        public string ImageCover { get; set; }
        public CourseState State { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
    }
    
}
