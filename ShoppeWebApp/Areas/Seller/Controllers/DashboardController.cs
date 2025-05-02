using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Seller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;

            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            // Nếu không chọn ngày, mặc định lấy 30 ngày gần nhất
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now;
                startDate = endDate.Value.AddDays(-30);
            }

            // Lấy các chi tiết đơn hàng trong khoảng thời gian lọc
            var orderDetails = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation) // Tải thông tin sản phẩm
                .Include(ct => ct.IdDonHangNavigation) // Tải thông tin đơn hàng
                .Where(ct => ct.IdSanPhamNavigation != null &&
                             ct.IdSanPhamNavigation.IdCuaHang == shopId &&
                             ct.IdDonHangNavigation != null &&
                             ct.IdDonHangNavigation.ThoiGianTao >= startDate &&
                             ct.IdDonHangNavigation.ThoiGianTao <= endDate)
                .ToList();

            // Tính tổng doanh thu theo sản phẩm
            var revenueByProduct = orderDetails
                .Where(ct => ct.IdSanPhamNavigation != null && ct.IdDonHangNavigation != null) // Đảm bảo không null
                .GroupBy(ct => new { ct.IdSanPhamNavigation.TenSanPham }) // Nhóm theo tên sản phẩm
                .Select(g => new RevenueByProduct
                {
                    ProductName = g.Key.TenSanPham,
                    TotalRevenue = g.Sum(ct => ct.SoLuong * ct.DonGia) // Tính tổng doanh thu
                })
                .OrderByDescending(r => r.TotalRevenue) // Sắp xếp theo doanh thu giảm dần
                .ToList();

            // Tính tổng doanh thu theo ngày
            var revenueByDay = orderDetails
                .Where(ct => ct.IdDonHangNavigation != null)
                .GroupBy(ct => ct.IdDonHangNavigation.ThoiGianTao.Value.Date)
                .Select(g => new RevenueByDay
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(ct => ct.SoLuong * ct.DonGia)
                })
                .OrderBy(r => r.Date)
                .ToList();

            // Tính tần suất đặt đơn hàng
            var orderFrequency = orderDetails
                .Where(ct => ct.IdDonHangNavigation != null)
                .GroupBy(ct => ct.IdDonHangNavigation.ThoiGianTao.Value.Date)
                .Select(g => new OrderFrequency
                {
                    Date = g.Key,
                    OrderCount = g.Select(ct => ct.IdDonHang).Distinct().Count()
                })
                .OrderBy(f => f.Date)
                .ToList();

            // Tạo ViewModel
            var viewModel = new DashboardViewModel
            {
                DonChoXacNhan = _context.Chitietdonhangs
                    .Include(ct => ct.IdDonHangNavigation)
                    .Where(ct => ct.IdSanPhamNavigation != null &&
                                 ct.IdSanPhamNavigation.IdCuaHang == shopId &&
                                 ct.IdDonHangNavigation != null &&
                                 ct.IdDonHangNavigation.TrangThai == Constants.CHO_XAC_NHAN)
                    .Select(ct => ct.IdDonHang)
                    .Distinct()
                    .Count(),
                DonDaXacNhan = _context.Chitietdonhangs
                    .Include(ct => ct.IdDonHangNavigation)
                    .Where(ct => ct.IdSanPhamNavigation != null &&
                                 ct.IdSanPhamNavigation.IdCuaHang == shopId &&
                                 ct.IdDonHangNavigation != null &&
                                 ct.IdDonHangNavigation.TrangThai == Constants.DA_XAC_NHAN)
                    .Select(ct => ct.IdDonHang)
                    .Distinct()
                    .Count(),
                DonDaGiao = _context.Chitietdonhangs
                    .Include(ct => ct.IdDonHangNavigation)
                    .Where(ct => ct.IdSanPhamNavigation != null &&
                                 ct.IdSanPhamNavigation.IdCuaHang == shopId &&
                                 ct.IdDonHangNavigation != null &&
                                 ct.IdDonHangNavigation.TrangThai == Constants.DA_GIAO)
                    .Select(ct => ct.IdDonHang)
                    .Distinct()
                    .Count(),
                DonHuy = _context.Chitietdonhangs
                    .Include(ct => ct.IdDonHangNavigation)
                    .Where(ct => ct.IdSanPhamNavigation != null &&
                                 ct.IdSanPhamNavigation.IdCuaHang == shopId &&
                                 ct.IdDonHangNavigation != null &&
                                 ct.IdDonHangNavigation.TrangThai == Constants.HUY_DON_HANG)
                    .Select(ct => ct.IdDonHang)
                    .Distinct()
                    .Count(),
                SanPhamTamKhoa = _context.Sanphams
                    .Count(s => s.TrangThai == Constants.TAM_KHOA && s.IdCuaHang == shopId),
                SanPhamHetHang = _context.Sanphams
                    .Count(s => s.SoLuongKho == 0 && s.IdCuaHang == shopId),
                RevenueByDay = revenueByDay,
                RevenueByProduct = revenueByProduct,
                OrderFrequency = orderFrequency,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };

            return View(viewModel);
        }
    }
}