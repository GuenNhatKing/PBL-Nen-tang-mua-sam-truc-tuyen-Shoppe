using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Customer;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;
        public ProductController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> index(string? id = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Sanphams
            .FirstOrDefaultAsync(i => i.IdSanPham == id);
            if (product == null)
            {
                return NotFound();
            }
            await _context.Entry(product).Reference(i => i.IdDanhMucNavigation).LoadAsync();
            await _context.Entry(product).Reference(i => i.IdCuaHangNavigation).LoadAsync();
            var productPageInfo = new ProductPageInfo
            {
                IdDanhMuc = product.IdDanhMuc,
                IdCuaHang = product.IdCuaHang,
                IdSanPham = product.IdSanPham,
                TenSanPham = product.TenSanPham,
                UrlAnh = product.UrlAnh,
                MoTa = product.MoTa,
                SoLuongKho = product.SoLuongKho,
                GiaGoc = product.GiaGoc,
                GiaBan = product.GiaBan,
                TongDiemDanhGia = product.TongDiemDanhGia,
                SoLuongDanhGia = product.SoLuongDanhGia,
                SoLuongBan = product.SoLuongBan,
                TenDanhMuc = product.IdDanhMucNavigation.TenDanhMuc,
                TenCuaHang = product.IdCuaHangNavigation.TenCuaHang,
                UrlAnhCuaHang = product.IdCuaHangNavigation.UrlAnh,
                SoSanPhamDangBan = await _context.Sanphams.CountAsync(i => i.IdCuaHang == product.IdCuaHang),
                ThoiGianThamGia = product.IdCuaHangNavigation.ThoiGianTao,
                SdtCuaHang = product.IdCuaHangNavigation.Sdt,
                DiaChiCuaHang = product.IdCuaHangNavigation.DiaChi,
                DanhSachDanhGia = await _context.Danhgia
                .Include(i => i.IdNguoiDungNavigation)
                .Where(i => i.IdSanPham == id)
                .Select(i => new DanhGiaSanPham
                {
                    IdNguoiDung = i.IdNguoiDung,
                    UrlAnhNguoiDung = i.IdNguoiDungNavigation.UrlAnh,
                    DiemDanhGia = i.DiemDanhGia,
                    NoiDung = i.NoiDung,
                    TenNguoiDung = i.IdNguoiDungNavigation.HoVaTen,
                    ThoiGianDg = i.ThoiGianDg,
                }).ToListAsync()
            };
            return View(productPageInfo);
        }
        public async Task<JsonResult> AddProductToShoppingCart(string? IdSanPham, int? SoLuong)
        {
            if (IdSanPham == null || SoLuong == null)
            {
                return Json(new JSResult(false, null));
            }
            var IdNguoiDung = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (IdNguoiDung == null)
            {
                return Json(new JSResult(false, null));
            }
            var sanPham = await _context.Sanphams.FirstOrDefaultAsync(i => i.IdSanPham == IdSanPham);
            var nguoiDung = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == IdNguoiDung);
            if(sanPham == null || nguoiDung == null)
            {
                return Json(new JSResult(false, null));
            }
            var gioHang = await _context.Giohangs.FirstOrDefaultAsync(i => i.IdSanPham == sanPham.IdSanPham && i.IdNguoiDung == nguoiDung.IdNguoiDung);
            if(gioHang == null)
            {
                if(SoLuong < 0 || SoLuong > sanPham.SoLuongKho)
                {
                    return Json(new JSResult(false, null, "Số lượng không hợp lệ"));
                }
                var newItem = new Giohang
                {
                    IdNguoiDung = nguoiDung.IdNguoiDung,
                    IdSanPham = sanPham.IdSanPham,
                    SoLuong = SoLuong ?? 0,
                };
                try
                {
                    _context.Giohangs.Add(newItem);
                    await _context.SaveChangesAsync();
                }
                catch(Exception)
                {
                    return Json(new JSResult(false, null));
                }
            }
            else
            {
                int SoLuongMoi = (SoLuong ?? 0) + gioHang.SoLuong;
                if(SoLuongMoi < 0 || SoLuongMoi > sanPham.SoLuongKho)
                {
                    return Json(new JSResult(false, null, "Số lượng không hợp lệ"));
                }
                gioHang.SoLuong = SoLuongMoi;
                try
                {
                    _context.Giohangs.Update(gioHang);
                    await _context.SaveChangesAsync();
                }
                catch(Exception)
                {
                    return Json(new JSResult(false, null));
                }
            }
            return Json(new JSResult(true, null));
        }
    }
}
