using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Admin;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace ShoppeWebApp.Areas.Admin.Controllers
{
    [Authorize("Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public DashboardController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy dữ liệu từ cơ sở dữ liệu
            var model = new HomeViewModel
            {
                TotalUsers = _context.Nguoidungs.Count(nd => nd.VaiTro == 1), 
                TotalSellers = _context.Nguoidungs.Count(nd => nd.VaiTro == 2), 
                TotalOrders = _context.Donhangs.Count(), 
                TotalProducts = _context.Sanphams.Count() 
            };

            return View(model);
        }
        
    }
}