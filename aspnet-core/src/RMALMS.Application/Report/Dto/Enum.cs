using RMALMS.Paging;
using System;

namespace RMALMS.Reports.Dto
{

    public enum ActionReport : int
    {
        User_Activities = 1,
        User_Activities_Course_enrolment_and_re__enrolment = 2,
        User_Activities_Start_and_Resume_the_course = 3,
        User_Activities_Course_page_viewed = 4,
        User_Activities_Quiz_attempted = 5,
        User_Activities_Assignment_attempted = 6,
        User_Activities_Viewed_on_course_announcement = 7,
        User_Activities_Add_or_remove_bookmark_on_course_page = 8,
        User_Activities_Viewed_on_QA = 9,
        User_Activities_Post_and_reply_on_QA = 10,
        User_Activities_Admin_Settings = 11,
        User_Activities_Report_Downloaded = 12,

        Student_Statistics = 20,
        Student_Statistics_Course_Completion = 21,
        Student_Statistics_Course_Final_Exam_Result = 22,
        Student_Statistics_Course_Assignment_completed = 23,
        Student_Statistics_Course_Quiz_Completion = 24,
        Student_Statistics_Course_Total_Scores = 25,

        Instructor_Statistics = 30,
        Instructor_Statistics_Report_download = 31,
        Instructor_Statistics_Created_Course = 32,
        Instructor_Statistics_Published_course = 33,
        Instructor_Statistics_Course_enrolled_and_invited_student = 34,
        Instructor_Statistics_Update_student_performance = 35,
        Instructor_Statistics_Course_Features = 36,

        Course_Statistics = 40,
        Course_Statistics_Published_Course = 41,
        Course_Statistics_Course_Instructors = 42,
        Course_Statistics_Course_Announcement = 43,
        Course_Statistics_Course_Options_and_Features = 44,

        Course_Statistics_Course_enrolled_and_invited_student = 45,
        Course_Statistics_Update_student_performance = 46,

        Course_Import_Export = 50,
        Course_Import_Export_Course_Import = 51,
        Course_Import_ExportCourse_Export = 52,

    }

    public enum ServiceName : byte
    {
        Abp_AspNetCore_Mvc_Controllers_AbpUserConfigurationController = 1,
        RMALMS_Annoucements_AnnoucementAppService = 2,
        RMALMS_Assignments_AssignmentAppService = 3,
        RMALMS_Authorization_Accounts_AccountAppService = 4,
        RMALMS_Categories_CategoryAppService = 5,
        RMALMS_CertificationTemplates_CertificationTemplateAppService = 6,
        RMALMS_Configuration_ConfigurationAppService = 7,
        RMALMS_Controllers_TokenAuthController = 8,
        RMALMS_CourseCategories_CourseCategoryAppService = 9,
        RMALMS_Courses_CourseAppService = 10,
        RMALMS_Courses_CourseGroupAppService = 11,
        RMALMS_Courses_UserAssignedToCourses = 12,
        RMALMS_CourseSettings_CourseInstanceAppService = 13,
        RMALMS_GradeSchemes_GradeSchemeAppService = 14,
        RMALMS_Groups_GroupAppService = 15,
        RMALMS_Languages_LanguagesAppService = 16,
        RMALMS_Modules_ModuleAppService = 17,
        RMALMS_Pages_PageAppService = 18,
        RMALMS_QAQuestions_QAQuestionAppService = 19,
        RMALMS_Questions_QuestionAppService = 20,
        RMALMS_Quizzes_QuizAppService = 21,
        RMALMS_Reports_ReportAppService = 22,
        RMALMS_StudentProgresses_StudentProgressAppService = 23,
        RMALMS_TimeZone_TimeZoneAppService = 1,
        RMALMS_UserExtraRoles_UserExtraRoleAppService = 24,
        RMALMS_UserGroups_UserGroupAppService = 25,
        RMALMS_Users_UserAppService = 26,
        RMALMS_UserStatus_UserStatusAppService = 27,
        RMALMS_Web_Host_Controllers_HomeController = 28,
    }
}
