using Newtonsoft.Json;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMALMS.Courses.Dto
{
    //public class StudentAssignmentQuizDto
    //{
    //    public long StudentId { get; set; }
    //    //public float? Point { get; set; }
    //    [JsonIgnore]
    //    public List<SettingScore> AssignmentQuizGroup { get; set; }
    //    [JsonIgnore]
    //    public List<SettingScore> EveryOneAssignmentQuiz { get; set; }
    //    public List<SettingScore> AssignmentQuizzes
    //    {
    //        get
    //        {
    //            var result = AssignmentQuizGroup ?? new List<SettingScore>();
    //            if (EveryOneAssignmentQuiz != null)
    //            {
    //                var others = EveryOneAssignmentQuiz.Except(AssignmentQuizGroup);
    //                if (others != null && others.Count() > 0)
    //                {
    //                    result.AddRange(others);
    //                }
    //            }

    //            return result;
    //        }
    //    }
    //}

    //public class SettingScore
    //{
    //    public float? Point { get; set; }
    //    /// <summary>
    //    /// using for both QuizSettingId, AssignmentSettingId
    //    /// </summary>
    //    public Guid SettingId { get; set; }
    //    public Guid PageId { get; set; }
    //}
}
