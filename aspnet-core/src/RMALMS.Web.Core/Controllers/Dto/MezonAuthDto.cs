
namespace RMALMS.Controllers.Dto
{
    public class MezonAuthDto
    {
        public string AuthCode { get; set; }
        public string RedirectUri { get; set; }
        public string TenancyName { get; set; }
    }
}