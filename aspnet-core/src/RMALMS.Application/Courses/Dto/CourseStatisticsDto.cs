using Abp.Application.Services.Dto;
using RMALMS.Anotations;
using RMALMS.Assignments.Dto;
using RMALMS.Entities;
using RMALMS.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Courses.Dto
{
    public class CourseStatisticsDto
    {
        public GridResult<StudentDto> Students { get; set; }
        public int TotalPage { get; set; }
        //public float TotalScore { get; set; }
        public List<SQuizDto> Quizzes { get; set; }
        public List<SAssignmentDto> Assignments { get; set; }
        public float?[,] StudentQuizScores { get; set; }
        public float?[,] StudentAssignmentScores { get; set; }
        public List<StudentAssignmentDto> StudentAssignments { get; set; }
    }

    public class StudentDto {
        public long StudentId { get; set; }
        [ApplySearchAttribute]
        public string Name { get; set; }
        public int NCompletedPage { get; set; }
        public float Score { get; set; }//score that student got
        public float TotalScore { get; set; }// total score = sum (quiz score, assignment score that assigned to student)        
        public bool IsDoneSurvey { get; set; }
        public Guid CourseAssignedStudentId { get; set; }
        public AssignedStatus Status { get; set; }
        public DateTime CourseAssignedStudentTime { get; set; }
        public int EnrollCount { get; set; }
    }

    public class SQuizDto: EntityDto<Guid> {
        public string Name { get; set; }
        public float Score { get; set; }
        public Entities.QuizScoreToKeepType ScoreToKeepType { get; set; }
        public Entities.QuizType QuizType { get; set; }
    }

    public class SAssignmentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public float Score { get; set; }
        public DisplayGradeType DisplayGrade { get; set; }
        public bool IsGroupAssignment { get; set; }
        public bool IsAssignIndividualGrade { get; set; }
    }


    public class EditStudentStatisticDto {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public int NCompletedPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalScore { get; set; }
        public bool Survey { get; set; }

    }
}
