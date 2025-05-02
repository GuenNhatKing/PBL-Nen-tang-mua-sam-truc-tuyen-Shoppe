using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private ShoppeWebAppDbContext _context;
        public ShoppingCartController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
