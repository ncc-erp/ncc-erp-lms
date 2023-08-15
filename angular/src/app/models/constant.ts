export enum PERMISSION {
    TENANTS = 'Pages.Tenants',
    COURSE = 'Pages.Course',
    SYSTEMS = 'Pages.Users',
    STUDENTS = 'Pages.Student'
}
export enum PermissonConstants {
    Pages_Tenants = 'Pages.Tenants',
    Pages_Users = 'Pages.Users',
    Pages_Roles = 'Pages.Roles',
    Pages_Groups = 'Pages.Groups',
    // tenant permission
    Pages_Courses = 'Pages.Course',
    Pages_AccountCeritification = 'Pages.Certification',
    Pages_Dashboard = 'Pages.Dashboard',
    Pages_CourseView = 'Pages.CourseView',
    Pages_Calendar = 'Pages.Calendar',
    Pages_Settings = 'Pages.Settings',
    Pages_UserGroups = 'Pages.UserGroups',
    Pages_Report = 'Pages.Report',
    // for report
    Reports_UserLoginAll = 'Reports.UserLoginAll',
    Reports_UserLoginStudent = 'Reports.UserLoginStudent',
    Reports_UserActivitiesAll = 'Reports.UserActivitiesAll',
    Reports_UserActivitiesStudent = 'Reports.UserActivitiesStudent',
    Reports_StudentStatistics = 'Reports.StudentStatistics',
    Reports_StudentStatisticsInCourse = 'Reports.StudentStatisticsInCourse',
    Reports_CourseStatisticsAll = 'Reports.CourseStatisticsAll',
    Reports_CourseStatisticsSelfCreated = 'Reports.CourseStatisticsSelfCreated',
    Reports_CourseImportExportAll = 'Reports.CourseImportExport',
    Reports_CourseImportExportSelfCreated = 'Reports.CourseImportExportSelfCreated',
    InstuctorStatistics = 'Reports.InstructorStatistics',
}

export enum RoleConstants {
    Admin = 'System Administrator',
    CourseAdmin = 'Course Administrator',
    Student = 'Student'
}

export enum StudentDefaultViewName {
    Dashboard = 0,
    Courses = 1,
    Calendar = 2,
}

export enum DashboardDefaultViewName {
    CardView = 0,
    ListView = 1
}
