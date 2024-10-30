using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Scorms.Dto
{
    public class ScormStatisticDto
    {
        public List<StudentQuizDto> Quizzes { get; set; }
        public List<StudentStatistic> StudentsResult { get; set; }
    }
    public class StudentQuizDto 
    {
        public string Name { get; set; }
        public float Score { get; set; }
    }
    public class StudentQuizScore
    {
        public string Name { get; set; }
        public float Score { get; set; }
    }

    public class StudentStatistic
    {
        public long StudentId { get; set; }
        public string Name { get; set; }
        public List<float> QuizzesScore { get; set; }//score that student got
        public float Progress { get; set; }

    }
    public class StudentScormResultDto
    {
        public long StudentId { get; set; }
        public string Name { get; set; }
        public string QuizName { get; set; }
        public float QuizzesScore { get; set; }//score that student got
    }
    public class StudentScormGradeDto
    {
        public List<StudentQuizDto> Quizzes { get; set; }
        public StudentStatistic StudentStatistic { get; set; }
        public List<StudentStatistic> StudentsProgress { get; set; }
    }
}
