using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Talent.Dto
{
    public class TalentCourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string RelationInfo { get; set; }
        public string ImageCover { get; set; }
        public string FullPathImageCover { get => RMALMSConsts.ServerRootAddress + ImageCover; }
    }
}
