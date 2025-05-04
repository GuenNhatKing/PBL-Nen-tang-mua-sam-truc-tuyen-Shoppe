using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Customer;

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
        public async Task<IActionResult> Index()
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return NotFound();
            }
            var allShops = await _context.Giohangs
            .Include(i => i.IdSanPhamNavigation)
            .Where(i => i.IdNguoiDung == userId)
            .GroupBy(i => new
            {
                IdCuaHang = i.IdSanPhamNavigation.IdCuaHang,
            })
            .Select(i => i.Key.IdCuaHang)
            .ToListAsync();
            var shoppingCart = new ShoppingCartViewModel();
            foreach (var i in allShops)
            {
                shoppingCart.danhSachCuaHang.Add(new ShoppingCartShopProducts
                {
                    IdCuaHang = i,
                    TenCuaHang = (await _context.Cuahangs.FirstOrDefaultAsync(j => j.IdCuaHang == i))?.TenCuaHang
                });
            }
            foreach(var i in shoppingCart.danhSachCuaHang)
            {
                i.danhSachSanPham = await _context.Giohangs
                .Include(j => j.IdSanPhamNavigation)
                .Where(j => j.IdSanPhamNavigation.IdCuaHang == i.IdCuaHang && j.IdNguoiDung == userId)
                .Select(j => new ShoppingCartProductInfo
                {
                    IdSanPham = j.IdSanPham,
                    TenSanPham = j.IdSanPhamNavigation.TenSanPham,
                    UrlAnh = j.IdSanPhamNavigation.UrlAnh,
                    GiaBan = j.IdSanPhamNavigation.GiaBan,
                    GiaGoc = j.IdSanPhamNavigation.GiaGoc,
                    SoLuong = j.SoLuong,
                    SoLuongKho = j.IdSanPhamNavigation.SoLuongKho,
                })
                .ToListAsync();
            }
            return View(shoppingCart);
        }

        public async Task<JsonResult> UpdateQuantity(string? IdSanPham, int? SoLuong)
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
                return Json(new JSResult(true, null));
            }
            else
            {
                int SoLuongMoi = SoLuong ?? 0;
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

        public async Task<IActionResult> ConfirmPayment(string[] danhSachSanPham)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return NotFound();
            }
            var allShops = await _context.Giohangs
            .Include(i => i.IdSanPhamNavigation)
            .Where(i => i.IdNguoiDung == userId && danhSachSanPham.Contains(i.IdSanPham))
            .GroupBy(i => new
            {
                IdCuaHang = i.IdSanPhamNavigation.IdCuaHang,
            })
            .Select(i => i.Key.IdCuaHang)
            .ToListAsync();
            var shoppingCart = new ShoppingCartViewModel();
            foreach (var i in allShops)
            {
                shoppingCart.danhSachCuaHang.Add(new ShoppingCartShopProducts
                {
                    IdCuaHang = i,
                    TenCuaHang = (await _context.Cuahangs.FirstOrDefaultAsync(j => j.IdCuaHang == i))?.TenCuaHang
                });
            }
            foreach(var i in shoppingCart.danhSachCuaHang)
            {
                i.danhSachSanPham = await _context.Giohangs
                .Include(j => j.IdSanPhamNavigation)
                .Where(j => j.IdSanPhamNavigation.IdCuaHang == i.IdCuaHang && j.IdNguoiDung == userId && danhSachSanPham.Contains(j.IdSanPham))
                .Select(j => new ShoppingCartProductInfo
                {
                    IdSanPham = j.IdSanPham,
                    TenSanPham = j.IdSanPhamNavigation.TenSanPham,
                    UrlAnh = j.IdSanPhamNavigation.UrlAnh,
                    GiaBan = j.IdSanPhamNavigation.GiaBan,
                    GiaGoc = j.IdSanPhamNavigation.GiaGoc,
                    SoLuong = j.SoLuong,
                    SoLuongKho = j.IdSanPhamNavigation.SoLuongKho,
                })
                .ToListAsync();
            }
            return View(shoppingCart);
        }
    }
}
