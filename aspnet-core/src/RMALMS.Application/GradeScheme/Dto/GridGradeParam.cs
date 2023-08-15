using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Paging;

namespace RMALMS.GradeSchemes.Dto
{
    public class GridGradeParam
    {
        public GridParam input { get; set; }
        public Guid? courseId { get; set; }
    }
}
