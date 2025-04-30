using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Authorize(AuthenticationSchemes = "SellerSchema", Policy = "Seller")]
    [Area("Seller")]
    public class AllShopsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
