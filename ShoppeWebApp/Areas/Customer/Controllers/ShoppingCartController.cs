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
            var nguoiDung = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == userId);
            if (nguoiDung == null)
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
            foreach (var i in shoppingCart.danhSachCuaHang)
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
            if (sanPham == null || nguoiDung == null)
            {
                return Json(new JSResult(false, null));
            }
            var gioHang = await _context.Giohangs.FirstOrDefaultAsync(i => i.IdSanPham == sanPham.IdSanPham && i.IdNguoiDung == nguoiDung.IdNguoiDung);
            if (gioHang == null)
            {
                return Json(new JSResult(true, null));
            }
            else
            {
                int SoLuongMoi = SoLuong ?? 0;
                if (SoLuongMoi < 0 || SoLuongMoi > sanPham.SoLuongKho)
                {
                    return Json(new JSResult(false, null, "Số lượng không hợp lệ"));
                }
                gioHang.SoLuong = SoLuongMoi;
                try
                {
                    _context.Giohangs.Update(gioHang);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return Json(new JSResult(false, null));
                }
            }
            return Json(new JSResult(true, null));
        }

        public async Task<JsonResult> RemoveFromCart(string? IdSanPham)
        {
            if (IdSanPham == null)
            {
                return Json(new JSResult(false, null));
            }
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new JSResult(false, null));
            }
            var nguoiDung = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == userId);
            if (nguoiDung == null)
            {
                return Json(new JSResult(false, null));
            }
            try
            {
                var cart = await _context.Giohangs.FirstOrDefaultAsync(i => i.IdSanPham == IdSanPham && i.IdNguoiDung == userId);
                if (cart == null) return Json(new JSResult(false, null));
                _context.Giohangs.Remove(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Json(new JSResult(false, null));
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
            var nguoiDung = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == userId);
            if (nguoiDung == null)
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
            foreach (var i in shoppingCart.danhSachCuaHang)
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
            if(_context.Thongtinlienhes.FirstOrDefault(i => i.IdNguoiDung == userId) == null)
            {
                    string? maxIdLienHe = await _context.Thongtinlienhes.IgnoreQueryFilters().OrderByDescending(i => i.IdLienHe)
                        .Select(i => i.IdLienHe).FirstOrDefaultAsync();
                    string newIdLienHe = "";
                    if (maxIdLienHe == null)
                    {
                        newIdLienHe = new String('0', 10);
                    }
                    else
                    {
                        int? num = Convert.ToInt32(maxIdLienHe);
                        if (num == null) throw new InvalidDataException("Id khong dung dinh dang");
                        else
                        {
                            int newId = (int)num + 1;
                            newIdLienHe = newId.ToString("D10");
                        }
                    }

                var newLienHe = new Thongtinlienhe
                {
                    IdLienHe = newIdLienHe,
                    IdNguoiDung = nguoiDung.IdNguoiDung,
                    HoVaTen = nguoiDung.HoVaTen,
                    Sdt = nguoiDung.Sdt,
                    DiaChi = nguoiDung.DiaChi,
                };
                _context.Thongtinlienhes.Add(newLienHe);
                await _context.SaveChangesAsync();
            }
            shoppingCart.ThongTinLienHe = await _context.Thongtinlienhes.Where(i => i.IdNguoiDung == userId).ToListAsync();
            return View(shoppingCart);
        }
        [HttpPost]
        public async Task<JsonResult> Payment(string[] danhSachSanPham, string IdLienHe)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new JSResult(false, null));
            }
            var nguoiDung = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == userId);
            if (nguoiDung == null)
            {
                return Json(new JSResult(false, null));
            }

            var sanPhams = await _context.Sanphams
            .Where(i => danhSachSanPham.Contains(i.IdSanPham))
            .ToListAsync();
            decimal tongTien = 0;
            List<string> idCuaHangs = new List<string>();
            foreach (var i in sanPhams)
            {
                if (!idCuaHangs.Contains(i.IdCuaHang))
                {
                    idCuaHangs.Add(i.IdCuaHang);
                }
            }
            if (nguoiDung.SoDu < tongTien)
            {
                return Json(new JSResult(false, null, "Số dư tài khoản không đủ để thanh toán đơn hàng này!"));
            }
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var idch in idCuaHangs)
                    {
                        tongTien = 0;
                        string? maxIdDonHang = await _context.Donhangs.OrderByDescending(i => i.IdDonHang)
                            .Select(i => i.IdDonHang).FirstOrDefaultAsync();
                        string newIdDonHang = "";
                        if (maxIdDonHang == null)
                        {
                            newIdDonHang = new String('0', 10);
                        }
                        else
                        {
                            int? num = Convert.ToInt32(maxIdDonHang);
                            if (num == null) throw new InvalidDataException("Id khong dung dinh dang");
                            else
                            {
                                int newId = (int)num + 1;
                                newIdDonHang = newId.ToString("D10");
                            }
                        }
                        var donHang = new Donhang
                        {
                            IdDonHang = newIdDonHang,
                            IdLienHe = IdLienHe,
                            TongTien = tongTien,
                            TrangThai = Constants.CHO_XAC_NHAN,
                            ThoiGianTao = DateTime.UtcNow,
                        };
                        _context.Donhangs.Add(donHang);
                        await _context.SaveChangesAsync();
                        foreach (var i in sanPhams)
                        {
                            if(i.IdCuaHang == idch)
                            {
                                int soLuong = (await _context.Giohangs.FirstOrDefaultAsync(j => j.IdSanPham == i.IdSanPham && j.IdNguoiDung == userId))!.SoLuong;
                                var productDetails = new Chitietdonhang
                                {
                                    IdDonHang = donHang.IdDonHang,
                                    IdSanPham = i.IdSanPham,
                                    DonGia = i.GiaBan,
                                    SoLuong = soLuong,
                                };
                                tongTien += soLuong * i.GiaBan;
                                _context.Chitietdonhangs.Add(productDetails);
                            }
                        }
                        donHang.TongTien = tongTien;
                        _context.Donhangs.Update(donHang);
                        nguoiDung.SoDu -= tongTien;
                        _context.Nguoidungs.Update(nguoiDung);
                        await _context.SaveChangesAsync();
                    }

                    // Xoa khoi gio hang
                    var gioHangs = await _context.Giohangs
                        .Where(i => i.IdNguoiDung == userId && danhSachSanPham.Contains(i.IdSanPham))
                        .ToListAsync();
                    foreach (var i in gioHangs)
                    {
                        _context.Giohangs.Remove(i);
                    }
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await trans.RollbackAsync();
                }
            }
            return Json(new JSResult(true, null));
        }
    }
}
