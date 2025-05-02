using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Seller;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(AuthenticationSchemes = "SellerSchema", Roles = "Seller")]
    public class DashboardController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public DashboardController(ShoppeWebAppDbContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
            var shopName = User.Claims.FirstOrDefault(c => c.Type == "TenCuaHang")?.Value;

            if (string.IsNullOrEmpty(shopId) || string.IsNullOrEmpty(shopName))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new DashboardViewModel
            {
                DonChoXacNhan = _context.Donhangs.Count(d => d.TrangThai == Constants.CHO_XAC_NHAN && d.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == shopId)),
                DonDaXacNhan = _context.Donhangs.Count(d => d.TrangThai == Constants.DA_XAC_NHAN && d.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == shopId)),
                DonDaGiao = _context.Donhangs.Count(d => d.TrangThai == Constants.DA_GIAO && d.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == shopId)),
                DonHuy = _context.Donhangs.Count(d => d.TrangThai == Constants.HUY_DON_HANG && d.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == shopId)),
                SanPhamHetHang = _context.Sanphams.Count(s => s.SoLuongKho == 0 && s.IdCuaHang == shopId),
                SanPhamTamKhoa = _context.Sanphams.Count(s => s.ThoiGianXoa != null && s.IdCuaHang == shopId)
            };

            return View(viewModel);
        }
    }
}