using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ShoppeWebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View("AccessDenied");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
