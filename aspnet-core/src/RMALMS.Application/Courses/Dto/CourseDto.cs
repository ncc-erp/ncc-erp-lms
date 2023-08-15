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
    public class CourseDto : EntityDto<Guid>
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? StatusId { get; set; }
        public CourseType Type { get; set; }
        public string ImageCover { get; set; }
        public string RelatedInformation { get; set; }
        public Guid CourseId { get; set; }
        public string Level { get; set; }
        public CourseState State { get; set; }
        public IFormFile File { get; set; }
        public string RelatedImage { get; set; }
        public IFormFile RelatedFile { get; set; }
        public virtual int? LanguageId { get; set; }
        public string Identifier { get; set; }
        public bool StudentCanOnlyParticipiateCourseBetweenTheseDate { get; set; }
        public bool RestrictStudentFromViewThisCourseAfterEndDate { get; set; }
        public bool RestrictStudentsFromViewingThisCourseBeforeEndDate { get; set; }
    }

    public class CourseDashboardDto : EntityDto<Guid>
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        [ApplySearch]
        public string Name { get; set; }
        public string Description { get; set; }
        public string RelationInfo { get; set; }
        public int CurrentPoint { get; set; }
        public int ComminSoonDatePoint { get; set; }
        public bool AlreadyStart { get; set; }
        public bool IsCommingSoon { get; set; }
        public bool IsSelfPaced { get; set; }
        public bool IsUpComming { get; set; }
        public string ImageCover { get; set; }
        public AssignedStatus? Status { get; set; }
        public CourseState State { get; set; }
        public bool IsLearning { get { var now = DateTime.UtcNow; return this.StartDate.HasValue && this.StartDate <= now && (!this.EndDate.HasValue || now <= this.EndDate); } }

        public bool IsOwner { get; set; }

        public int NCompletedPage { get; set; }
        public int TotalPage { get; set; }
        public int CompletedPercent
        {
            get
            {
                if (TotalPage > 0)
                    return NCompletedPage * 100 / TotalPage;
                else return 0;
            }
        }
        public bool IsArchived { get; set; }
    }

    public class CourseDetailDto : EntityDto<Guid>
    {
        public string Title { get; set; }
    }

    public class StudentCourseDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseInstanceId { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public string RelatedInformation { get; set; }
        public string ImageCover { get; set; }
        public int CompletedPercent
        {
            get
            {
                if (TotalPage > 0)
                    return NCompletedPage * 100 / TotalPage;
                else return 0;
            }
        }
        public AssignedStatus State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int NCompletedPage { get; set; }
        public int TotalPage { get; set; }
    }
}
