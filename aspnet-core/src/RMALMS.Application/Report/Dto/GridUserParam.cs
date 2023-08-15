using RMALMS.Paging;
using System;

namespace RMALMS.Reports.Dto
{
    public class GridParams : GridParam
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? UserId { get; set; }

    }
}
