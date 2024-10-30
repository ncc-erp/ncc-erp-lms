using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Entities;
using RMALMS.Paging;

namespace RMALMS.Quizzes.Dto
{
    public class GridQuizzesParam
    {
        public GridParam input { get; set; }
        public Guid courseId { get; set; }
        public QuizType? quiztype { get; set; }
    }
}
