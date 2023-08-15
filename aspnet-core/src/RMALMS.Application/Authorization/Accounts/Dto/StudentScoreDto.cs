using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Authorization.Accounts.Dto
{
    public class StudentScoreDto
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public float StudentScore { get; set; }
        public float TotalScore { get; set; }
    }    
}
