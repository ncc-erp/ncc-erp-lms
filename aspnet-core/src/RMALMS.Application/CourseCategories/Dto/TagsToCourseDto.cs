using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.CourseCategories.Dto
{
    public class TagsToCourseDto
    {
        [Required]
        public Guid CourseId { get; set; }
        public Guid[] Tags { get; set; }
    }
}
