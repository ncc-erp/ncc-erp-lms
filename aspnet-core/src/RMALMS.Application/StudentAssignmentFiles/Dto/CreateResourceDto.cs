using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using RMALMS.Anotations;
using RMALMS.Entities;

namespace RMALMS.StudentAssignmentFiles.Dto
{
    [AutoMapTo(typeof(StudentAssignmentFile))]
    public class CreateStudentAssignmentFileDto : EntityDto<Guid>
    {
        public string FileName { get; set; }
        public Guid? CourseGroupId { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public IFormFile File { get; set; }
    }
}
