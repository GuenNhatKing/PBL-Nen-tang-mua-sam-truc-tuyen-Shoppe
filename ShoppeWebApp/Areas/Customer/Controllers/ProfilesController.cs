﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using QLTTDT.Services;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Customer;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer, Admin")]
    public class ProfilesController : Controller
    {
        private ShoppeWebAppDbContext _context;
        private IWebHostEnvironment _webHost;
        public ProfilesController(ShoppeWebAppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index()
        {
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new Profiles
            {
                IdNguoiDung = user.IdNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                SoDienThoai = user.Sdt,
                UrlAnhDaiDien = user.UrlAnh,
                Cccd = user.Cccd,
                DiaChi = user.DiaChi,
                SoDu = user.SoDu,
            };
            return View(profiles);
        }
        public async Task<IActionResult> ChangeProfiles(string? username, int? id)
        {
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new Profiles
            {
                IdNguoiDung = user.IdNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                SoDienThoai = user.Sdt,
                UrlAnhDaiDien = user.UrlAnh,
                Cccd = user.Cccd,
                DiaChi = user.DiaChi,
                SoDu = user.SoDu,
            };
            return View(profiles);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeProfiles(Profiles profiles, IFormFile? AnhDaiDien = null)
        {
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    user.HoVaTen = profiles.HoVaTen;
                    user.Cccd = profiles.Cccd;
                    user.Sdt = profiles.SoDienThoai;
                    user.Email = profiles.Email;
                    user.DiaChi = profiles.DiaChi;
                    user.SoDu = profiles.SoDu;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhDaiDien!, new[] { "images", "UserAvatar" }))
                    {
                        imageUpload.DeleteImage(user.UrlAnh!);
                        user.UrlAnh = imageUpload.FilePath;
                    }
                    _context.Nguoidungs.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction("Index");
            }
            return View(profiles);
        }

        public async Task<IActionResult> ManageProducts(int? TinhTrang = null)
        {
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new ManageProducts
            {
                IdNguoiDung = user.IdNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                UrlAnhDaiDien = user.UrlAnh,
            };
            var query = _context.Donhangs
                .Include(i => i.IdLienHeNavigation)
                .IgnoreQueryFilters()
                .Where(i => i.IdLienHeNavigation.IdNguoiDung == user.IdNguoiDung)
                .AsQueryable();
            if (TinhTrang != null)
            {
                query = query.Where(i => i.TrangThai == TinhTrang);
            }
            var donHangs = await query.OrderByDescending(i => i.ThoiGianTao).ToListAsync();
            foreach (var donHang in donHangs)
            {
                var order = new ManageProductOrder
                {
                    IdDonHang = donHang.IdDonHang,
                    ThoiGianTao = donHang.ThoiGianTao,
                    ThoiGianGiao = donHang.ThoiGianGiao,
                    TinhTrang = donHang.TrangThai,
                    TongTien = donHang.TongTien,
                };
                var dsIdCuaHang = await _context.Chitietdonhangs
                    .Where(i => i.IdDonHang == donHang.IdDonHang)
                    .Include(i => i.IdSanPhamNavigation)
                    .GroupBy(i => new
                    {
                        IdCuaHang = i.IdSanPhamNavigation.IdCuaHang
                    })
                    .Select(i => i.Key.IdCuaHang)
                    .ToListAsync();
                var dsCuaHang = await _context.Cuahangs.Where(i => dsIdCuaHang.Contains(i.IdCuaHang)).ToListAsync();
                foreach (var cuaHang in dsCuaHang)
                {
                    var chiTietCuaHang = new OrderDescCuaHang
                    {
                        IdCuaHang = cuaHang.IdCuaHang,
                        TenCuaHang = cuaHang.TenCuaHang
                    };
                    var dsSanPham = await _context.Chitietdonhangs
                        .Include(i => i.IdSanPhamNavigation)
                        .Where(i => i.IdDonHang == donHang.IdDonHang && i.IdSanPhamNavigation.IdCuaHang == cuaHang.IdCuaHang)
                        .Select(i => new
                        {
                            IdSanPham = i.IdSanPham,
                            TenSanPham = i.IdSanPhamNavigation.TenSanPham,
                            UrlAnhSanPham = i.IdSanPhamNavigation.UrlAnh,
                            DonGia = i.DonGia,
                            SoLuong = i.SoLuong,
                        })
                        .ToListAsync();
                    foreach (var sanPham in dsSanPham)
                    {
                        var chiTietSanPham = new OrderDescSanPham
                        {
                            IdSanPham = sanPham.IdSanPham,
                            TenSanPham = sanPham.TenSanPham,
                            UrlAnhSanPham = sanPham.UrlAnhSanPham,
                            SoLuong = sanPham.SoLuong,
                            DonGia = sanPham.DonGia,
                        };
                        var dsDanhGia = await _context.Danhgia
                            .Where(i => i.IdSanPham == sanPham.IdSanPham && i.IdNguoiDung == user.IdNguoiDung)
                            .ToListAsync();
                        foreach (var danhGia in dsDanhGia)
                        {
                            chiTietSanPham.danhGias.Add(new DanhGiaInfo
                            {
                                IdDanhGia = danhGia.IdDanhGia,
                                ThoiGianDanhGia = danhGia.ThoiGianDg,
                                NoiDungDanhGia = danhGia.NoiDung,
                                SoSaoDanhGia = danhGia.DiemDanhGia
                            });
                        }
                        chiTietCuaHang.danhSachSanPhams.Add(chiTietSanPham);
                    }
                    order.danhSachCuaHang.Add(chiTietCuaHang);
                }
                profiles.danhSachDonHang.Add(order);
            }
            ViewBag.TinhTrang = TinhTrang;
            return View(profiles);
        }
        public async Task<IActionResult> HuyDonHang(string? IdDonHang)
        {
            if(String.IsNullOrEmpty(IdDonHang))
            {
                return BadRequest();
            }
            var donHang = await _context.Donhangs.FirstOrDefaultAsync(i => i.IdDonHang == IdDonHang);
            string? IdNguoiDung = GetUserId();
            if(donHang != null && IdNguoiDung != null)
            {
                var nguoiDung = await _context.Nguoidungs
                    .FirstOrDefaultAsync(i => i.IdNguoiDung == IdNguoiDung);
                nguoiDung.SoDu += donHang.TongTien;
                donHang.TrangThai = ShoppeWebApp.Data.Constants.HUY_DON_HANG;
                _context.Donhangs.Update(donHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageProducts");
        }
        public async Task<IActionResult> DaNhanDonHang(string? IdDonHang)
        {
            if(String.IsNullOrEmpty(IdDonHang))
            {
                return BadRequest();
            }
            var donHang = await _context.Donhangs.FirstOrDefaultAsync(i => i.IdDonHang == IdDonHang);
            if(donHang != null)
            {
                donHang.TrangThai = ShoppeWebApp.Data.Constants.DA_GIAO;
                donHang.ThoiGianGiao = DateTime.Now;
                _context.Donhangs.Update(donHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageProducts");
        }
        public async Task<JsonResult> SubmitRating(string? IdSanPham, int? ratingValue, string? ratingContent)
        {
            if (IdSanPham == null || ratingValue == null)
            {
                return Json(new JSResult(false, null));
            }
            var sanPham = await _context.Sanphams.FirstOrDefaultAsync(i => i.IdSanPham == IdSanPham);
            if (sanPham == null)
            {
                return Json(new JSResult(false, null));
            }
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return Json(new JSResult(false, null));
            }
            string? maxIdDanhGia = await _context.Danhgia.OrderByDescending(i => i.IdDanhGia)
                .Select(i => i.IdDanhGia).FirstOrDefaultAsync();
            Console.WriteLine();
            string newIdDanhGia = "";
            if (maxIdDanhGia == null)
            {
                newIdDanhGia = new String('0', 10);
            }
            else
            {
                int? num = Convert.ToInt32(maxIdDanhGia);
                if (num == null) throw new InvalidDataException("Id khong dung dinh dang");
                else
                {
                    int newId = (int)num + 1;
                    newIdDanhGia = newId.ToString("D10");
                }
            }
            var newDanhGia = new Danhgia
            {
                IdDanhGia = newIdDanhGia,
                IdNguoiDung = user.IdNguoiDung,
                IdSanPham = sanPham.IdSanPham,
                ThoiGianDg = DateTime.Now,
                DiemDanhGia = (int)ratingValue,
                NoiDung = ratingContent,
            };
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Danhgia.Add(newDanhGia);
                    await _context.SaveChangesAsync();
                    sanPham.SoLuongDanhGia += 1;
                    sanPham.TongDiemDanhGia += newDanhGia.DiemDanhGia;
                    _context.Sanphams.Update(sanPham);
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                catch (Exception)
                {
                    await trans.RollbackAsync();
                    return Json(new JSResult(false, null));
                }
            }
            return Json(new JSResult(true, null));
        }

        public async Task<IActionResult> ManageAddress()
        {
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new ManageAddress
            {
                IdNguoiDung = user.IdNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                UrlAnhDaiDien = user.UrlAnh,
            };
            profiles.danhSachLienHe = await _context.Thongtinlienhes
                .Where(i => i.IdNguoiDung == user.IdNguoiDung).ToListAsync();
            return View(profiles);
        }
        [HttpPost]
        public async Task<JsonResult> AddAddressInfo(string? HoVaTen = null, string? SDT = null, string? DiaChi = null)
        {
            if (HoVaTen == null || SDT == null || DiaChi == null)
            {
                return Json(new JSResult(false, null));
            }
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return Json(new JSResult(false, null));
            }

            try
            {
                string? maxIdLienHe = await _context.Thongtinlienhes.IgnoreQueryFilters().OrderByDescending(i => i.IdLienHe)
                .Select(i => i.IdLienHe).FirstOrDefaultAsync();
                Console.WriteLine();
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
                var lienHe = new Thongtinlienhe
                {
                    IdLienHe = newIdLienHe,
                    HoVaTen = HoVaTen,
                    IdNguoiDung = user.IdNguoiDung,
                    Sdt = SDT,
                    DiaChi = DiaChi,
                };
                _context.Thongtinlienhes.Add(lienHe);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Json(new JSResult(false, null));
            }
            return Json(new JSResult(true, null));
        }
        [HttpPost]
        public async Task<JsonResult> UpdateAddressInfo(string? IdLienHe = null, string? HoVaTen = null, string? SDT = null, string? DiaChi = null)
        {
            if (IdLienHe == null || HoVaTen == null || SDT == null || DiaChi == null)
            {
                return Json(new JSResult(false, null));
            }
            var lienHe = await _context.Thongtinlienhes.FirstOrDefaultAsync(i => i.IdLienHe == IdLienHe);
            if (lienHe == null)
            {
                return Json(new JSResult(false, null));
            }
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return Json(new JSResult(false, null));
            }
            if (lienHe.IdNguoiDung != user.IdNguoiDung)
            {
                return Json(new JSResult(false, null));
            }
            try
            {
                lienHe.HoVaTen = HoVaTen;
                lienHe.Sdt = SDT;
                lienHe.DiaChi = DiaChi;
                _context.Thongtinlienhes.Update(lienHe);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Json(new JSResult(false, null));
            }
            return Json(new JSResult(true, null));
        }
        [HttpPost]
        public async Task<JsonResult> RemoveAddressInfo(string? IdLienHe = null)
        {
            if (IdLienHe == null)
            {
                return Json(new JSResult(false, null));
            }
            var lienHe = await _context.Thongtinlienhes.FirstOrDefaultAsync(i => i.IdLienHe == IdLienHe);
            if (lienHe == null)
            {
                return Json(new JSResult(false, null));
            }
            string? currUserId = GetUserId();
            var user = await _context.Nguoidungs.FirstOrDefaultAsync(i => i.IdNguoiDung == currUserId);
            if (user == null)
            {
                return Json(new JSResult(false, null));
            }
            if (lienHe.IdNguoiDung != user.IdNguoiDung)
            {
                return Json(new JSResult(false, null));
            }
            try
            {
                lienHe.DaXoa = true;
                _context.Thongtinlienhes.Update(lienHe);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Json(new JSResult(false, null));
            }
            return Json(new JSResult(true, null));
        }

        private string? GetUserId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }
    }
}
