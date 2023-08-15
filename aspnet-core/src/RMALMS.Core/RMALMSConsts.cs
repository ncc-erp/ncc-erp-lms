namespace RMALMS
{
    public class RMALMSConsts
    {
        public const string LocalizationSourceName = "RMALMS";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;
        public const string DefaultTenantName = "NCC";

        public static string ServerRootAddress { get; set; }
        public static bool IsEnableMultiTenant { get; set; } = false;
        public static string SercurityCode { get; set; } = "12345678";
        public static string NCCEmployeeGroupId { get; set; } = "69bc56d4-08a9-4f0c-3382-08dabe69b67f";
        public static string CourseInstanceId { get; set; } = "1cc6bd7c-99e0-4427-5cd6-08dabbfb453e";

    }
}
