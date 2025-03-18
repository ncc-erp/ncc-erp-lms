
namespace RMALMS.Authorization.Dto
{
    public class MezonHashAuthDto
    {
        public string HashData { get; set; }
        public string TenancyName { get; set; } = "NCC";
    }

    public class BaseHashData {
        public string query_id { get; set; }
        public string user { get; set; }
        public long auth_date { get; set; }
        public string signature { get; set; }
    }
    public class HashData : BaseHashData {
        public string hash { get; set; }
    }


}