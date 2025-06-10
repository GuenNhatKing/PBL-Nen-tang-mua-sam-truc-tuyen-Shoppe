using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Admin;
using System.Linq;

namespace ShoppeWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public OrderController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm, int page = 1, int pageSize = 10)
        {
            var query = _context.Donhangs
                .Select(o => new
                {
                    o.IdDonHang,
                    o.IdLienHe,
                    o.ThoiGianTao,
                    o.TongTien,
                    o.TrangThai
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.IdDonHang.Contains(searchTerm) || 
                    _context.Thongtinlienhes
                        .IgnoreQueryFilters()
                        .Where(t => t.IdLienHe == o.IdLienHe)
                        .Select(t => t.HoVaTen)
                        .FirstOrDefault()
                        .Contains(searchTerm));
            }

            var totalItems = query.Count();

            var orders = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList() 
                .Select(o => new OrderViewModel
                {
                    MaDonHang = o.IdDonHang,
                    TenKhachHang = _context.Thongtinlienhes
                        .IgnoreQueryFilters()
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

            return View(orders);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
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
                        .IgnoreQueryFilters()
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
                                .FirstOrDefault(),
                            DanhGia = _context.Danhgia
                                .Where(d => d.IdSanPham == c.IdSanPham && c.IdDonHang == o.IdDonHang)
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
                        .ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult Cancel(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
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
                        .IgnoreQueryFilters()
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