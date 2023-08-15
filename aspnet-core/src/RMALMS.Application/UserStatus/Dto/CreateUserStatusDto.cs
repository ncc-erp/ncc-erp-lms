using Abp.AutoMapper;
using Abp.Domain.Entities;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.UserStatus.Dto
{
    [AutoMapTo(typeof(Entities.UserStatus))]
    public class CreateUserStatusDto
    {
        public int Level { get; set; }
        public string DisplayName { get; set; }
        public bool IsStatic { get; set; }
        public CompareOperation? LowCompareOperation { get; set; }
        public int? RequiredNumber { get; set; }
    }
}
