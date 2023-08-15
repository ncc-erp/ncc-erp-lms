using Abp.MultiTenancy;
using System.Collections.Generic;
using static RMALMS.Authorization.Roles.StaticRoleNames;

namespace RMALMS.Authorization
{
    public static class PermissionNames
    {
        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Users = "Pages.Users";
        public const string Pages_Roles = "Pages.Roles";
        public const string Pages_Groups = "Pages.Groups";

        // tenant permission
        public const string Pages_Courses = "Pages.Course";
        public const string Pages_AccountCeritification = "Pages.Certification";
        public const string Pages_Dashboard = "Pages.Dashboard";
        public const string Pages_CourseView = "Pages.CourseView";
        public const string Pages_Calendar = "Pages.Calendar";
        public const string Pages_Settings = "Pages.Settings";
        public const string Pages_UserGroups = "Pages.UserGroups";
        public const string Pages_Report = "Pages.Report";
        public const string Pages_Categories = "Pages.Categories";
        public const string Pages_Account = "Pages.Account";
        public const string Pages_Configurations = "Pages.Configurations";


        // for report
        public const string Reports_UserLoginAll = "Reports.UserLoginAll";
        public const string Reports_UserLoginStudent = "Reports.UserLoginStudent";
        public const string Reports_UserActivitiesAll = "Reports.UserActivitiesAll";
        public const string Reports_UserActivitiesStudent = "Reports.UserActivitiesStudent";
        public const string Reports_StudentStatistics = "Reports.StudentStatistics";
        public const string Reports_StudentStatisticsInCourse = "Reports.StudentStatisticsInCourse";
        public const string Reports_CourseStatisticsAll = "Reports.CourseStatisticsAll";
        public const string Reports_CourseStatisticsSelfCreated = "Reports.CourseStatisticsSelfCreated";
        public const string Reports_CourseImportExportAll = "Reports.CourseImportExport";
        public const string Reports_CourseImportExportSelfCreated = "Reports.CourseImportExportSelfCreated";
        public const string InstuctorStatistics = "Reports.InstructorStatistics";
    }

    public class SystemPermission
    {
        public string Permission { get; set; }
        public MultiTenancySides MultiTenancySides { get; set; }
        public string DisplayName { get; set; }
        public bool IsConfiguration { get; set; }

        public static List<SystemPermission> ListPermissions = new List<SystemPermission>()
        {
            // for default
            new SystemPermission{ Permission =  PermissionNames.Pages_Tenants, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Tenants" },
            new SystemPermission{ Permission =  PermissionNames.Pages_Users, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Users" },
            new SystemPermission{ Permission =  PermissionNames.Pages_Roles, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Roles" },
            new SystemPermission{ Permission =  PermissionNames.Pages_Account, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Account" },

            //new SystemPermission{ Permission =  PermissionNames.Pages_Groups, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Groups" },

            // tenant
            new SystemPermission{ Permission =  PermissionNames.Pages_Courses, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course"},
            new SystemPermission{ Permission =  PermissionNames.Pages_AccountCeritification, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Account - Certification", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Dashboard, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Dashboard", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_CourseView, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Calendar, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Calendar", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Settings, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Settings", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_UserGroups, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "User & Groups", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Report, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Reports", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Categories, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Categories", IsConfiguration = true },
            //new SystemPermission{ Permission =  PermissionNames.Pages_Account, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Account", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Configurations, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Configuration", IsConfiguration = true },

            // report
            new SystemPermission{ Permission =  PermissionNames.Reports_UserLoginAll, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "User Login (All)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_UserLoginStudent, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "User Login (Student)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_UserActivitiesAll, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "User Activities (All)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_UserActivitiesStudent, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "User Activities (Student)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_StudentStatistics, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Student Statistics (All)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_StudentStatisticsInCourse, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Student Statistics (In Course)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_CourseStatisticsAll, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course Statistics (All)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_CourseStatisticsSelfCreated, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course Statistics (Self-Created)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_CourseImportExportAll, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course Import/Export (All)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Reports_CourseImportExportSelfCreated, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Course Import/Export (Self-Created)", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.InstuctorStatistics, MultiTenancySides = MultiTenancySides.Tenant, DisplayName = "Instructor Statistics", IsConfiguration = true },

        };
    }

    public class GrantPermissionRoles
    {
        public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>()
        {
            // For System Admin
            {
                Tenants.Admin,
                new List<string>()
                {
                    PermissionNames.Pages_Account,
                    PermissionNames.Pages_Courses,
                    PermissionNames.Pages_Categories,
                    PermissionNames.Pages_Configurations,
                    PermissionNames.Pages_Settings,
                    PermissionNames.Pages_UserGroups,
                    PermissionNames.Pages_Report,

                    PermissionNames.Reports_CourseImportExportAll,
                    PermissionNames.Reports_CourseImportExportSelfCreated,
                    PermissionNames.Reports_CourseStatisticsAll,
                    PermissionNames.Reports_CourseStatisticsSelfCreated,
                    PermissionNames.Reports_StudentStatistics,
                    PermissionNames.Reports_StudentStatisticsInCourse,
                    PermissionNames.Reports_UserActivitiesAll,
                    PermissionNames.Reports_UserActivitiesStudent,
                    PermissionNames.Reports_UserLoginAll,
                    PermissionNames.Reports_UserLoginStudent
                }
            },

            // For Course Admin
            {
                Tenants.CourseAdmin,
                new List<string>()
                {
                    PermissionNames.Pages_Account,
                    PermissionNames.Pages_Courses,
                    PermissionNames.Pages_UserGroups,
                    PermissionNames.Pages_Report,

                    PermissionNames.Reports_CourseImportExportAll,
                    PermissionNames.Reports_CourseImportExportSelfCreated,
                    PermissionNames.Reports_CourseStatisticsAll,
                    PermissionNames.Reports_CourseStatisticsSelfCreated,
                    PermissionNames.Reports_StudentStatistics,
                    PermissionNames.Reports_StudentStatisticsInCourse,
                    PermissionNames.Reports_UserActivitiesAll,
                    PermissionNames.Reports_UserActivitiesStudent,
                    PermissionNames.Reports_UserLoginAll,
                    PermissionNames.Reports_UserLoginStudent
                }
            },

            // For Student
            {
                Tenants.Student,
                new List<string>()
                {
                    PermissionNames.Pages_Account,
                    PermissionNames.Pages_AccountCeritification,
                    PermissionNames.Pages_Dashboard,
                    PermissionNames.Pages_CourseView,
                    PermissionNames.Pages_Calendar,                    
                }
            },

        };
    }
}
