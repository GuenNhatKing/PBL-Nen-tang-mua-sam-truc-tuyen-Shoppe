using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Customer;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AllProductsController : Controller
    {
        private readonly ShoppeWebAppContext _context;
        public AllProductsController(ShoppeWebAppContext context)
        {
            _context = context;
        }
        public IActionResult Index(string IdDanhMuc)
        {
            AllProductInfo productInfos = new AllProductInfo(_context, IdDanhMuc);
            return View(productInfos);
        }
    }
}

