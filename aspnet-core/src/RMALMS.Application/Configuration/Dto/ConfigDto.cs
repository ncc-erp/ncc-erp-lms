using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Configuration.Dto
{
    public class ConfigDto
    {
        [Required]
        [StringLength(500)]
        public string Location { get; set; }
        public string ScormLocation { get; set; }
    }
}
