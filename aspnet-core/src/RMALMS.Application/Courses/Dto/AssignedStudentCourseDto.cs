using RMALMS.Anotations;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class AssignedStudentCourseDto
    {
        public Guid StudentAssignedCourseId { get; set; }
        public long StudentId { get; set; }
        [ApplySearch]
        public string StudentName { get; set; }
        [ApplySearch]
        public string UserName { get; set;}
        public IEnumerable<string> Roles { get; set; }
        public string RoleName { get { return Roles != null ? string.Join(", ", Roles) : string.Empty; } }
        public DateTime? LastActivity { get; set; }
        public long TotalActivity { get; set; }
        public int EnrollCount { get; set; }
        public AssignedStatus Status { get; set; }
    }
}
