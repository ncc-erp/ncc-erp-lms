using RMALMS.Paging;
using System;

namespace RMALMS.QAQuestions.Dto
{
    public class GridQAAnswerParam : GridDiscussionParam
    {        
        public bool IsFollower { get; set; } = false;
        public bool IsResponse { get; set; } = false;
        public string Sort { get; set; }
    }
    public class GridDiscussionParam
    {
        public GridParam input { get; set; }
        public Guid? courseInstanceId { get; set; }        
    }
}
