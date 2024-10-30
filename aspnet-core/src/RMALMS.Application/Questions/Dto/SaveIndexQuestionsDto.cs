using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Questions.Dto
{
    public class SaveIndexQuestionsDto
    {
        public List<IndexQuestion> ListChange { get; set; }
        public Guid QuizId { get; set; }
    }
    public class IndexQuestion
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
    }
}
