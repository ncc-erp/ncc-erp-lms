using Abp.Authorization.Roles;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RMALMS.GradeSchemes.Dto
{

    public class CreateGradeSchemeDto
    {

        public string Title { get; set; }
        public Guid CourseId { get; set; }
        public GradeSchemeStatus Status { get; set; }
        public List<GradeSchemeElementDto> ElementList { get; set; }
    }
}
