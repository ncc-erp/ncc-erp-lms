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
    public class LMSSettingInput 
    {
        public  string Key { get; set; }
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        //public string EntityType { get; set; }
        public string Value { get; set; }
        //public bool Checkbox { get; set; } = false;
    }
    public class LMSSettingOut
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
