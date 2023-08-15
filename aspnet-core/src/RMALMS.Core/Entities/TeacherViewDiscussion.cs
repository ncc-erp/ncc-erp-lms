using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Entities
{
    public class TeacherViewDiscussion : AuditedEntity<Guid>
    {
        public Guid QAId { get; set; }
        public string QAType { get; set; } // view for question or response
    }
}
