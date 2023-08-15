using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Paging;

namespace RMALMS.Resources.Dto
{
    public class GridResourceParam
    {
        public GridParam input { get; set; }
        public Guid? EntityId { get; set; }
        public string EntityType { get; set; }
    }
}
