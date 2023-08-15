using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Paging;

namespace RMALMS.Annoucements.Dto
{
    public class GridAnnoucementParam
    {
        public GridParam input { get; set; }
        public Guid? courseInstanceId { get; set; }
    }
}
