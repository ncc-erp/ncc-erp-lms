using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class StatisticGridParam :GridParam
    {
        public Guid courseInstanceId { get; set; }
    }
}
