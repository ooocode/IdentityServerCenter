using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Study.Website.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/",
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme,OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
