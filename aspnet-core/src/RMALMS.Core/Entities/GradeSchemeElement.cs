using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class GradeSchemeElement : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(GradeSchemeId))]
        public GradeScheme GradeScheme { get; set; }
        public Guid GradeSchemeId { get; set; }
        public float LowRange { get; set; }
        public float HighRange { get; set; }
        public string Name { get; set; }
        public CompareOperation LowCompareOperation { get; set; }
        public CompareOperation HighCompareOpertion { get; set; }
    }

    public enum CompareOperation
    {
        LessEqual = 0,
        LessThan = 1,
        GreaterEqual = 2,
        GreaterThan =3,
        Equal = 4
    }
}
