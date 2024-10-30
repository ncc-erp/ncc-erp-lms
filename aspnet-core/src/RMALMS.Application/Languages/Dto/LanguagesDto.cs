using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Languages.Dto
{
    public class LanguagesDto : Entity<int>
    {
        public string DisplayName { get; set; }
    }
}
