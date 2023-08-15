using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMALMS.DomainServices.Entity
{
    public class StudentAssignedQuizAssignment
    {
        public long StudentId { get; set; }
        public Guid CourseAssignedStudentId { get; set; }

        public List<SettingScore> AssignedByQuizGroups { get; set; }
        
        public List<SettingScore> EveryOneAssignmentQuizzes { get; set; }
        public List<SettingScore> AssignedList
        {
            get
            {
                var result = AssignedByQuizGroups != null ? AssignedByQuizGroups.ToList() : new List<SettingScore>();
                if (EveryOneAssignmentQuizzes != null)
                {
                    
                    var others = EveryOneAssignmentQuizzes.Where(e => !AssignedByQuizGroups.Select(s => s.PageId).Contains(e.PageId));
                    if (others != null && others.Count() > 0)
                    {
                        result.AddRange(others);
                    }
                }

                return result;
            }
        }
    }

    public class SettingScore
    {
        public float? Point { get; set; }
        /// <summary>
        /// using for both QuizSettingId, AssignmentSettingId
        /// </summary>
        public Guid SettingId { get; set; }
        public Guid PageId { get; set; }
    }
}
