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
    public class EditCourseDto: EntityDto<Guid>
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? LevelId { get; set; }
        public CourseType Type { get; set; }
        public string ImageCover { get; set; }
        public string RelatedInformation { get; set; }        
        public IFormFile File { get; set; }
        public string RelatedImage { get; set; }
        public IFormFile RelatedFile { get; set; }
        //public bool EnableCourseGradingScheme { get; set; }
        public virtual int? LanguageId { get; set; }
        public string Identifier { get; set; }
        public bool StudentCanOnlyParticipiateCourseBetweenTheseDate { get; set; }
        public bool RestrictStudentFromViewThisCourseAfterEndDate { get; set; }
        public bool RestrictStudentsFromViewingThisCourseBeforeEndDate { get; set; }
        public string Syllabus { get; set; }

        public int NPageCompleted { get; set; }
        public int TotalPage { get; set; }

        public CourseState State { get; set; }
        public CourseSource Sourse { get; set; }
        public string SoursePath { get; set; }

    }
    public class EditCourseSyllabusInput : EntityDto<Guid>
    {
        public string Syllabus { get; set; }
    }
}
