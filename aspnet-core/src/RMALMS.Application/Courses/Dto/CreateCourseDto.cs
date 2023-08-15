using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Courses.Dto
{
    [AutoMapTo(typeof(Entities.Course))]
    public class CreateCourseDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? LevelId { get; set; }
        public CourseType Type { get; set; }
        public string ImageCover { get; set; }//path to file
        public IFormFile File { get; set; }
        public IFormFile FileSCORM { get; set; }
        public CourseSource Sourse { get; set; }
    }
}
