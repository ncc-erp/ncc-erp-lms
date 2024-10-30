namespace RMALMS.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "System Administrator";
            public const string CourseAdmin = "Course Administrator";
            public const string Student = "Student";

        }
    }
}
