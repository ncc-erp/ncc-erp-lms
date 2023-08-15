using RMALMS.Entities;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class RequestAssignedStudentCourseDto
    {
        [Required]
        public Guid CourseInstanceId { get; set; }
        [Required]
        public AssignedStatus Status { get; set; }
        public GridParam Request { get; set; }
    }

    public class InvitationCourseRequestDto
    {
        [Required]
        public Guid CourseInstanceId { get; set; }
        public GridParam Request { get; set; }
    }

    public class UnAssignedStudentCourseDto
    {
        [Required]
        public Guid CourseInstanceId { get; set; }
        public GridParam Request { get; set; }
    }
}
