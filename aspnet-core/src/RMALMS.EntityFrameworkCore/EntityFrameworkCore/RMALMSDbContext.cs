using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.MultiTenancy;
using System.Text.RegularExpressions;
using RMALMS.Entities;

namespace RMALMS.EntityFrameworkCore
{
    public class RMALMSDbContext : AbpZeroDbContext<Tenant, Role, User, RMALMSDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<RMALMS.Entities.Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseContent> CourseContents { get; set; }
        public DbSet<GroupAssignedCourse> GroupAssignedCourses { get; set; }
        public DbSet<CourseInstance> CourseInstances { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<MediaContent> MediaContents { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<StudentProgress> StudentProgresses { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<UserExtraRole> UserExtraRoles { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<CourseAssignedStudent> CourseAssignedStudents { get; set; }
        public DbSet<UserLink> UserLinks { get; set; }
        public DbSet<Annoucement> Annoucements { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<UserTimeZone> UserTimeZones { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuestionQuiz> QuestionQuizzes { get; set; }
        public DbSet<GroupAssingedAssignment> GroupAssingedAssignments { get; set; }
        public DbSet<StudentAssingedAssignment> StudentAssingedAssignments { get; set; }
        public DbSet<UserCertification> UserCertifications { get; set; }
        public DbSet<StudentAssingedQuiz> StudentAssingedQuizzes { get; set; }
        public DbSet<GroupAssingedQuiz> GroupAssingedQuizzes { get; set; }
        public DbSet<AssignmentSetting> AssignmentSettings { get; set; }
        public DbSet<GradeScheme> GradeSchemes { get; set; }
        public DbSet<GradeSchemeElement> GradeSchemeElements { get; set; }
        public DbSet<QuizSetting> QuizSettings { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<StudentCourseGroup> StudentCourseGroups { get; set; }
        public DbSet<LMSSetting> LMSSettings { get; set; }
        public DbSet<QAQuestion> QAQuestions { get; set; }
        public DbSet<QAAnswer> QAAnswers { get; set; }
        public DbSet<UserFollowQAQuestion> UserFollowQAs { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<PageLinkExam> PageLinkExams { get; set; }
        public DbSet<FAQQuestion> FAQQuestions { get; set; }
        public DbSet<FAQAnswer> FAQAnswers { get; set; }
        public DbSet<TeacherViewDiscussion> TeacherViewDiscussions { get; set; }
        public DbSet<CourseCertificationTemplate> CourseCertificationTemplates { get; set; }
        public DbSet<CertificationTemplateGradeScheme> CertificationTemplateGradeSchemes { get; set; }
        public DbSet<UserBookMark> UserBookMarks { get; set; }
        public DbSet<CourseColor> CourseColors { get; set; }
        public DbSet<StudentAssignmentFile> StudentAssignmentFiles { get; set; }
        public DbSet<StudentSurvey> StudentSurveys { get; set; }
        public DbSet<StudentScorm> StudentScorms { get; set; }
        public DbSet<ScormTestAttempt> ScormTestAttempts { get; set; }
        public DbSet<StudentProgressScorm> StudentProgressScorms { get; set; }

        public RMALMSDbContext(DbContextOptions<RMALMSDbContext> options)
            : base(options)
        {
        }
    }
}
