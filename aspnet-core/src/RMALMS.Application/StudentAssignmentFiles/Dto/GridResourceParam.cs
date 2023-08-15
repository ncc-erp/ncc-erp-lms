using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Paging;

namespace RMALMS.StudentAssignmentFiles.Dto
{
    public class GridResourceParam
    {
        public GridParam input { get; set; }
        public Guid AssignmentSettingId { get; set; }
        public string EntityType { get; set; }
    }
}
