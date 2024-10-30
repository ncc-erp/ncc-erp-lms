namespace RMALMS.Configuration
{
    public static class AppSettingNames
    {
        public const string UiTheme = "App.UiTheme";


        // Tenant setting
        public const string CourceFolder = "Tenant.StorageLocation";
        public const string SCORMCourceResourceFolder = "Tenant.StorageScormLocation";
        public const string DashboardViewByStudent = "Tenant.DashboardViewByStudent";
        public const string CalendarViewByStudent = "Tenant.CalendarViewByStudent";
        public const string CourseEnrollRequireApproval = "Tenant.CourseEnrollRequireApproval";
        public const string ProficiencyLevelRequiredForEnroll = "Tenant.ProficiencyLevelRequiredForEnroll";
        public const string StudentDefaultView = "Tenant.StudentDefaultView";
        public const string DashboardDefaultView = "Tenant.DashboardDefaultView";
        public const string StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor = "Tenant.StudentCourseEnrollmentRequiredApprovalFromTheAdminInstructor";
        public const string StudentProficiencyLevelRequired = "Tenant.StudentProficiencyLevelRequired";

        // User setting here
        public const string UserPersonalInfoViewByPublic = "User.PersonalInfoViewByPublic";
        public const string UserPersonalLinksViewByPublic = "User.PersonalLinkViewByPublic";
        public const string ClientAppId = "App.ClientAppId";
        //Setting hệ thống
        public const string TimeScanFinishCourse = "TimeScanFinishCourse";
    }

    public enum StudentDefaultViewName
    {
        Dashboard = 0,
        Courses = 1,
        Calendar = 2,
    }

    public enum DashboardDefaultViewName
    {
        CardView = 0,
        ListView = 1
    }
}
