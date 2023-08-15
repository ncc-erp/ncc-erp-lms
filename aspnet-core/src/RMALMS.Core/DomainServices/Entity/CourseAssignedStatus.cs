using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.DomainServices.Entity
{
    public class CourseAssignedStatus
    {
        public CourseInstance CourseInstance { get; set; }
        public AssignedStatus? AssignedStatus { get; set; }        
    }
}
