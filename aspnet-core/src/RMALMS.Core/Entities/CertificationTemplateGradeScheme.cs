using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RMALMS.Entities
{
    public class CertificationTemplateGradeScheme : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid CourseCertificationTemplateId { get; set; }
        [ForeignKey(nameof(CourseCertificationTemplateId))]
        public CourseCertificationTemplate CourseCertificationTemplate { get; set; }
        public Guid GradeSchemeElementId { get; set; }
        [ForeignKey(nameof(GradeSchemeElementId))]
        public GradeSchemeElement GradeSchemeElement { get; set; }
    }
}
