using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Entities;
using RMALMS.Paging;

namespace RMALMS.Assignments.Dto
{
    public class GridAssignmentsParam
    {
        public GridParam input { get; set; }
        public Guid courseId { get; set; }
        public Guid? courseInstanceId { get; set; }
    }
}
