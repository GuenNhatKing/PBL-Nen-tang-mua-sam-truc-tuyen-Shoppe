using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Seller;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Area("Seller")]
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
            var viewModel = new DashboardViewModel
            {
                DonChoXacNhan = _context.Donhangs.Count(d => d.TrangThai == Constants.CHO_XAC_NHAN),
                DonDaXacNhan = _context.Donhangs.Count(d => d.TrangThai == Constants.DA_XAC_NHAN),
                DonDaGiao = _context.Donhangs.Count(d => d.TrangThai == Constants.DA_GIAO),
                DonHuy = _context.Donhangs.Count(d => d.TrangThai == Constants.HUY_DON_HANG),
                SanPhamHetHang = _context.Sanphams.Count(s => s.SoLuongKho == 0),
                SanPhamTamKhoa = _context.Sanphams.Count(s => s.ThoiGianXoa != null) 
            };

            return View(viewModel);
        }
    }
}