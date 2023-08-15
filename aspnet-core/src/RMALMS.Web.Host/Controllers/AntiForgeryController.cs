using Microsoft.AspNetCore.Antiforgery;
using RMALMS.Controllers;

namespace RMALMS.Web.Host.Controllers
{
    public class AntiForgeryController : RMALMSControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
