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
            Console.WriteLine($"ID: {id}");
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
    }
}
