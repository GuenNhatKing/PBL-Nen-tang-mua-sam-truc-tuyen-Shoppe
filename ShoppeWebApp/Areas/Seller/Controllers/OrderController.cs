using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Seller;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(AuthenticationSchemes = "SellerSchema", Roles = "Seller")]
    public class OrderController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public OrderController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        // Phương thức lấy ID cửa hàng của người bán hiện tại
        private string GetCurrentShopId()
        {
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
            
            if (string.IsNullOrEmpty(shopId))
            {
                // Log thông báo
                Console.WriteLine("Không tìm thấy IdCuaHang trong claims của người dùng");
                return null;
            }
            
            return shopId;
        }

        public IActionResult Index(string searchTerm, int page = 1, int pageSize = 10)
        {
            // Lấy ID cửa hàng hiện tại
            var shopId = GetCurrentShopId();
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Lấy danh sách ID đơn hàng có chứa sản phẩm thuộc shop hiện tại
            var ordersWithSellerProducts = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation)
                .Where(ct => ct.IdSanPhamNavigation != null && 
                       ct.IdSanPhamNavigation.IdCuaHang == shopId)
                .Select(ct => ct.IdDonHang)
                .Distinct()
                .ToList();

            // Nếu không có đơn hàng nào
            if (!ordersWithSellerProducts.Any())
            {
                ViewData["NoOrders"] = true;
                return View(new List<OrderViewModel>());
            }

            // Lấy thông tin các đơn hàng thuộc về shop
            var query = _context.Donhangs
                .Where(o => ordersWithSellerProducts.Contains(o.IdDonHang))
                .Select(o => new
                {
                    o.IdDonHang,
                    o.IdLienHe,
                    o.ThoiGianTao,
                    o.TongTien,
                    o.TrangThai
                });

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.IdDonHang.Contains(searchTerm) || 
                    _context.Thongtinlienhes
                        .Where(t => t.IdLienHe == o.IdLienHe)
                        .Select(t => t.HoVaTen)
                        .FirstOrDefault()
                        .Contains(searchTerm));
            }

            var totalItems = query.Count();

            var orders = query
                .OrderByDescending(o => o.ThoiGianTao) // Sắp xếp đơn hàng mới nhất lên đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() 
                .Select(o => new OrderViewModel
                {
                    MaDonHang = o.IdDonHang,
                    TenKhachHang = _context.Thongtinlienhes
                        .Where(t => t.IdLienHe == o.IdLienHe)
                        .Select(t => t.HoVaTen)
                        .FirstOrDefault(),
                    NgayDat = o.ThoiGianTao ?? DateTime.MinValue,
                    TongTien = o.TongTien,
                    TrangThai = o.TrangThai switch
                    {
                        Constants.HUY_DON_HANG => "Đã hủy",
                        Constants.CHO_XAC_NHAN => "Chờ xác nhận",
                        Constants.DA_XAC_NHAN => "Đã xác nhận",
                        Constants.DA_GIAO => "Đã giao",
                        _ => "Không xác định"
                    }
                })
                .ToList();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchTerm"] = searchTerm;
            ViewData["TotalOrders"] = totalItems;
            ViewData["ShopId"] = shopId;

            return View(orders);
        }
        
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy ID cửa hàng hiện tại
            var shopId = GetCurrentShopId();
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Kiểm tra xem đơn hàng có chứa sản phẩm của cửa hàng này không
            bool orderContainsShopProducts = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation)
                .Any(ct => ct.IdDonHang == id && 
                           ct.IdSanPhamNavigation != null && 
                           ct.IdSanPhamNavigation.IdCuaHang == shopId);

            if (!orderContainsShopProducts)
            {
                return NotFound("Đơn hàng không chứa sản phẩm của cửa hàng bạn");
            }

            // Lấy chi tiết đơn hàng, chỉ lấy sản phẩm thuộc cửa hàng hiện tại
            var order = _context.Donhangs
                .Where(o => o.IdDonHang == id)
                .Select(o => new OrderDetailsViewModel
                {
                    MaDonHang = o.IdDonHang,
                    NgayDat = o.ThoiGianTao ?? DateTime.MinValue,
                    TongTien = 0, // Sẽ tính lại tổng tiền chỉ cho sản phẩm của cửa hàng
                    TrangThai = o.TrangThai == Constants.HUY_DON_HANG ? "Đã hủy" :
                                o.TrangThai == Constants.CHO_XAC_NHAN ? "Chờ xác nhận" :
                                o.TrangThai == Constants.DA_XAC_NHAN ? "Đã xác nhận" :
                                o.TrangThai == Constants.DA_GIAO ? "Đã giao" : "Không xác định",
                    ThongTinLienHe = _context.Thongtinlienhes
                        .Where(t => t.IdLienHe == o.IdLienHe)
                        .Select(t => new ThongTinLienHeViewModel
                        {
                            HoVaTen = t.HoVaTen,
                            SoDienThoai = t.Sdt,
                            DiaChi = t.DiaChi
                        })
                        .FirstOrDefault(),
                    SanPham = new List<SanPhamViewModel>() // Sẽ được điền sau
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            // Lấy chỉ các sản phẩm thuộc cửa hàng hiện tại
            order.SanPham = _context.Chitietdonhangs
                .Include(c => c.IdSanPhamNavigation)
                .Where(c => c.IdDonHang == id && 
                            c.IdSanPhamNavigation != null && 
                            c.IdSanPhamNavigation.IdCuaHang == shopId)
                .Select(c => new SanPhamViewModel
                {
                    TenSanPham = c.IdSanPhamNavigation.TenSanPham,
                    SoLuong = c.SoLuong,
                    DonGia = c.DonGia,
                    ThanhTien = c.SoLuong * c.DonGia,
                    UrlAnh = c.IdSanPhamNavigation.UrlAnh,
                    DanhGia = _context.Danhgia
                        .Where(d => d.IdSanPham == c.IdSanPham) // Removed the order ID filter
                        .Select(d => new DanhGiaViewModel
                        {
                            TenNguoiDung = _context.Nguoidungs
                                .Where(u => u.IdNguoiDung == d.IdNguoiDung)
                                .Select(u => u.HoVaTen)
                                .FirstOrDefault(),
                            DiemDanhGia = d.DiemDanhGia,
                            NoiDung = d.NoiDung,
                            ThoiGianDG = d.ThoiGianDg.HasValue 
                                ? d.ThoiGianDg.Value.ToString("dd/MM/yyyy HH:mm") 
                                : "Không xác định" 
                        })
                        .ToList()
                })
                .ToList();

            // Tính lại tổng tiền chỉ cho sản phẩm của shop
            order.TongTien = order.SanPham.Sum(p => p.ThanhTien);

            return View(order);
        }
        
        [HttpPost]
        public IActionResult Confirm(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Kiểm tra quyền xử lý đơn hàng này
            var shopId = GetCurrentShopId();
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool canManageOrder = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation)
                .Any(ct => ct.IdDonHang == id && 
                           ct.IdSanPhamNavigation != null && 
                           ct.IdSanPhamNavigation.IdCuaHang == shopId);

            if (!canManageOrder)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xác nhận đơn hàng này.";
                return RedirectToAction("Index");
            }

            var order = _context.Donhangs.FirstOrDefault(o => o.IdDonHang == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.TrangThai != Constants.CHO_XAC_NHAN)
            {
                TempData["ErrorMessage"] = "Chỉ có thể xác nhận các đơn hàng ở trạng thái 'Chờ xác nhận'.";
                return RedirectToAction("Index");
            }

            order.TrangThai = Constants.DA_XAC_NHAN;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được xác nhận thành công.";
            return RedirectToAction("Index");
        }

        public IActionResult Cancel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy ID cửa hàng hiện tại
            var shopId = GetCurrentShopId();
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Kiểm tra xem đơn hàng có chứa sản phẩm của cửa hàng này không
            bool orderContainsShopProducts = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation)
                .Any(ct => ct.IdDonHang == id && 
                           ct.IdSanPhamNavigation != null && 
                           ct.IdSanPhamNavigation.IdCuaHang == shopId);

            if (!orderContainsShopProducts)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền hủy đơn hàng này.";
                return RedirectToAction("Index");
            }

            var order = _context.Donhangs
                .Where(o => o.IdDonHang == id)
                .Select(o => new OrderDetailsViewModel
                {
                    MaDonHang = o.IdDonHang,
                    NgayDat = o.ThoiGianTao ?? DateTime.MinValue,
                    TongTien = o.TongTien,
                    TrangThai = o.TrangThai == Constants.HUY_DON_HANG ? "Đã hủy" :
                                o.TrangThai == Constants.CHO_XAC_NHAN ? "Chờ xác nhận" :
                                o.TrangThai == Constants.DA_XAC_NHAN ? "Đã xác nhận" :
                                o.TrangThai == Constants.DA_GIAO ? "Đã giao" : "Không xác định",
                    ThongTinLienHe = _context.Thongtinlienhes
                        .Where(t => t.IdLienHe == o.IdLienHe)
                        .Select(t => new ThongTinLienHeViewModel
                        {
                            HoVaTen = t.HoVaTen,
                            SoDienThoai = t.Sdt,
                            DiaChi = t.DiaChi
                        })
                        .FirstOrDefault(),
                    SanPham = _context.Chitietdonhangs
                        .Where(c => c.IdDonHang == o.IdDonHang)
                        .Select(c => new SanPhamViewModel
                        {
                            TenSanPham = _context.Sanphams
                                .Where(s => s.IdSanPham == c.IdSanPham)
                                .Select(s => s.TenSanPham)
                                .FirstOrDefault(),
                            SoLuong = c.SoLuong,
                            DonGia = c.DonGia,
                            ThanhTien = c.SoLuong * c.DonGia,
                            UrlAnh = _context.Sanphams
                                .Where(s => s.IdSanPham == c.IdSanPham)
                                .Select(s => s.UrlAnh)
                                .FirstOrDefault()
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            if (order.TrangThai == "Đã xác nhận" || order.TrangThai == "Đã giao" || order.TrangThai == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng đã được xác nhận hoặc đã giao.";
                return RedirectToAction("Index");
            }

            return View(order); 
        }

        [HttpPost]
        public IActionResult ConfirmCancel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Kiểm tra quyền xử lý đơn hàng này
            var shopId = GetCurrentShopId();
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool canManageOrder = _context.Chitietdonhangs
                .Include(ct => ct.IdSanPhamNavigation)
                .Any(ct => ct.IdDonHang == id && 
                           ct.IdSanPhamNavigation != null && 
                           ct.IdSanPhamNavigation.IdCuaHang == shopId);

            if (!canManageOrder)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền hủy đơn hàng này.";
                return RedirectToAction("Index");
            }

            var order = _context.Donhangs.FirstOrDefault(o => o.IdDonHang == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.TrangThai == Constants.DA_XAC_NHAN || order.TrangThai == Constants.DA_GIAO)
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng đã được xác nhận hoặc đã giao.";
                return RedirectToAction("Index");
            }

            order.TrangThai = Constants.HUY_DON_HANG;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            return RedirectToAction("Index");
        }
    }
}