using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Scorms.Dto
{
    public class CourseScormPageDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public int Level { get; set; }
        public bool IsDone { get; set; }
        public List<CourseScormPageDto> Children { get; set; }
    }
}
