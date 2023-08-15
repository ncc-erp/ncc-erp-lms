using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Paging;

namespace RMALMS.Questions.Dto
{
    public class GridCourseParam
    {
        public GridParam input { get; set; }
        public Guid? courseId { get; set; }
        public Guid? quizId { get; set; }
    }
}
