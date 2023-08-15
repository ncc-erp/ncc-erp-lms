using System;
using System.Collections.Generic;
using System.Text;
using RMALMS.Entities;
using RMALMS.Paging;

namespace RMALMS.CertificationTemplates.Dto
{
    public class GridTemplatesParam
    {
        public GridParam input { get; set; }
        public Guid courseId { get; set; }
        public Guid? courseInstanceId { get; set; }
    }
}
