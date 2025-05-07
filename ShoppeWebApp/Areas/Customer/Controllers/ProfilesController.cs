using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhDaiDien!, new [] {"images", "UserAvatar"}))
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
        private string? GetUserId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }
    }
}
