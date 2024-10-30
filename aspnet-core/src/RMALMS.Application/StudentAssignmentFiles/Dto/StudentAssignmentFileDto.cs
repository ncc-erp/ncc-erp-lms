using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Anotations;
using RMALMS.Entities;

namespace RMALMS.StudentAssignmentFiles.Dto
{
    [AutoMapTo(typeof(StudentAssignmentFile))]
    public class StudentAssignmentFileDto : EntityDto<Guid>
    {
        [ApplySearchAttribute]
        public string FileName { get; set; }
        public Guid? CourseGroupId { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public string FilePath { get; set; }
        public string MineType { get; set; }


    }
}
