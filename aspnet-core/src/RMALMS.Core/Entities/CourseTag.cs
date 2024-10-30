using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CourseTag : Entity<Guid>
    {
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
