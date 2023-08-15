using Abp.Domain.Entities;
using RMALMS.Paging;
using System;
using System.Collections.Generic;

namespace RMALMS.Reports.Dto
{
    public class ReportUserLoginDto
    {
        public long Id { get; set; }
        public string IpAddress { get; set; }
        public string CourseName { get; set; }
        public DateTime CreationTime { get; set; }
        public long? UserId { get; set; }
        public string Action { get; set; }

    }
    public class ReportUserLoginDto_Ext: ReportUserLoginDto
    {
        public string MethodName { get; set; }       
        public string ServiceName { get; set; }       
        public string Parameters { get; set; }
        public Guid LinkId { get; set; }
    }
    public class GroupUserLoginOut
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public IEnumerable<ReportUserLoginDto> Users { get; set; }
        public int CountLogin { get; set; }

    }

    public class ReportExportLogDto
    {
        //public Guid? CourseInstanceId { get; set; }
        public string actionName { get; set; }
    }
    public class CourseId_CourseInstanceId_Name{
        public Guid CourseId;
        public Guid CourseInstanceId;
        public string CourseName;
}
}