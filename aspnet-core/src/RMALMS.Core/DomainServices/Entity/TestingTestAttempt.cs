using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.DomainServices.Entity
{
    public class TestingTestAttempt
    {
        public long StudentId { get; set; }
        public Guid TestAttemptId { get; set; }
        public PageType PageType { get; set; }
        public DateTime CreationTime { get; set; }
        public int? TimeLimit { get; set; }
    }
}
