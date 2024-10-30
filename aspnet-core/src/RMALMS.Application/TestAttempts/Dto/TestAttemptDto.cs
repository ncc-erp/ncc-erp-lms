using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using RMALMS.Entities;
using RMALMS.Questions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.TestAttempts.Dto
{
    //[AutoMapTo(typeof(TestAttempt))]
    public class TestAttemptDto: EntityDto<Guid>
    {
        public Guid QuizSettingId { get; set; }
        public TestAttemptStatus Status { get; set; }
        public float? Score { get; set; }
        public int? TimeRemaining { get; set; }//second
        //public int Count { get; set; }
        public List<StudentAnswerDto> StudentAnswers { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public DateTime LastModificationTime { get; set; }
                
        public PageType Type { get; set; }
        public UserCertification UserCertification { get; set; }
        public float? MaxScore { get; set; }
    }


}
