using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Questions.Dto
{
    public class GridStudentAnswerDto
    {        
        public Guid TestAttempId { get; set; }
        public GridParam input { get; set; }
    }
}
