using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.DomainServices.Entity
{
    public class StudentQuizAssignmentScore
    {
        public Guid SettingId { get; set; }//QuizSettingId, AssignmentSettingId
        public Guid  CourseAssignedStudentId { get; set; }
        public float StudentScore { get; set; }
        public float TotalScore { get; set; }
    }    
}
