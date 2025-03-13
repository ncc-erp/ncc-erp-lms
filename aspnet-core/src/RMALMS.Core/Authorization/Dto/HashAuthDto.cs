
namespace RMALMS.Authorization.Dto
{
    public class MezonHashAuthDto
    {
        public string HashKey { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string TenancyName { get; set; } = "NCC";
    }


}