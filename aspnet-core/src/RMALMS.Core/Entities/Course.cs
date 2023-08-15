using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class Course : FullAuditedEntity<Guid>, IMayHaveTenant, IMayHaveVersion, IMayHaveIdentifier
    {
        public int? TenantId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(LevelId))]
        public CourseLevel Level { get; set; }
        public Guid? LevelId { get; set; }
        public string ImageCover { get; set; }
        public CourseType Type { get; set; }
        public string Version { get; set; }
        public string Identifier { get; set; }
        public string RelatedInformation { get; set; }
        public string RelatedImage { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public virtual ApplicationLanguage Language { get; set; }
        public virtual int? LanguageId { get; set; }
        public CourseState State { get; set; }
        public string Syllabus { get; set; }
        public bool StudentCanOnlyParticipiateCourseBetweenTheseDate { get; set; }
        public bool RestrictStudentFromViewThisCourseAfterEndDate { get; set; }
        public bool RestrictStudentsFromViewingThisCourseBeforeEndDate { get; set; }
        public CourseSource Sourse { get; set; }
        public string SoursePath { get; set; }
    }

    public enum CourseType : byte
    {
        Recur = 0,
        Perpetual = 1
    }

    public enum CourseState : byte
    {
        Draft = 0,
        Publish = 1,
        Archived = 2
    }

     public enum CourseSource: byte
    {
        RMA = 0,
        Scorm2004 = 1,
        Scorm12 = 2,
    }
}
