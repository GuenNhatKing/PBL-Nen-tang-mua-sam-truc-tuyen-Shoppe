using System.Collections.Specialized;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ShoppeWebAppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ViewModels.Customer.LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Taikhoan = _context.Taikhoans
                    .FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

                if (Taikhoan != null)
                {
                    _context.Entry(Taikhoan).Reference(i => i.IdNguoiDungNavigation).Load();
                    if (Taikhoan.IdNguoiDungNavigation.VaiTro == Constants.ADMIN_ROLE)
                    {
                        Console.WriteLine($"Dang nhap cho admin, id={Taikhoan.IdNguoiDung}");
                        var identity = ViewModels.Authentication.AuthenticationInfo.CreateAdminIdentity(Taikhoan.IdNguoiDung, Taikhoan.Username);
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(Constants.COOKIE_EXPIRY_DAYS), // 3 days
                        };
                        await HttpContext.SignInAsync("CustomerSchema", principal, properties);
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    if (Taikhoan.IdNguoiDungNavigation.VaiTro == Constants.CUSTOMER_ROLE)
                    {
                        Console.WriteLine($"Dang nhap cho customer, id={Taikhoan.IdNguoiDung}");
                        var identity = ViewModels.Authentication.AuthenticationInfo.CreateCustomerIdentity(Taikhoan.IdNguoiDung, Taikhoan.Username);
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(Constants.COOKIE_EXPIRY_DAYS), // 3 days
                        };
                        await HttpContext.SignInAsync("CustomerSchema", principal, properties);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nhập sai mật khẩu hoặc tài khoản.");
                }
            }

            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(ViewModels.Customer.RegisterViewModel model)
        {
            // Kiểm tra mật khẩu xác nhận
            if (model.MatKhau != model.XacNhanMatKhau)
            {
                ModelState.AddModelError(nameof(model.XacNhanMatKhau), "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            // Kiểm tra email đã tồn tại
            var existingEmail = _context.Nguoidungs.FirstOrDefault(u => u.Email == model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
                return View(model);
            }

            // Kiểm tra số điện thoại đã tồn tại
            var existingPhone = _context.Nguoidungs.FirstOrDefault(u => u.Sdt == model.Sdt);
            if (existingPhone != null)
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại đã được sử dụng.");
                return View(model);
            }

            // Kiểm tra số điện thoại phải đủ 10 số
            if (model.Sdt.Length != 10 || !model.Sdt.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại phải là 10 chữ số.");
                return View(model);
            }

            // Tạo ID bằng cách tìm ID lớn nhất hiện tại và tăng lên
            var maxId = _context.Nguoidungs
                .OrderByDescending(u => u.IdNguoiDung)
                .Select(u => u.IdNguoiDung)
                .FirstOrDefault();

            string newId;
            if (string.IsNullOrEmpty(maxId))
            {
                newId = "0000000001"; // Nếu chưa có ID nào, bắt đầu từ 0000000001
            }
            else
            {
                // Pls fix it
                newId = (long.Parse(maxId) + 1).ToString("D10"); // Tăng ID lên 1 và định dạng thành 10 chữ số
            }

            var Nguoidung = new Nguoidung
            {
                IdNguoiDung = newId,
                HoVaTen = model.HoVaTen,
                Email = model.Email,
                Sdt = model.Sdt,
                Cccd = "000000000000",
                DiaChi = "Chưa cập nhật",
                VaiTro = Constants.CUSTOMER_ROLE,
                TrangThai = 1,
                ThoiGianTao = DateTime.Now
            };

            var Taikhoan = new Taikhoan
            {
                Username = model.TenDangNhap,
                Password = model.MatKhau,
                IdNguoiDung = newId
            };

            try
            {
                _context.Nguoidungs.Add(Nguoidung);
                _context.Taikhoans.Add(Taikhoan);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký: " + ex.Message);
                return View(model);
            }
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ViewModels.Customer.ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Taikhoan = _context.Taikhoans
                        .Include(a => a.IdNguoiDungNavigation)
                        .FirstOrDefault(a => a.Username == model.Username && a.IdNguoiDungNavigation.Sdt == model.PhoneNumber);

                    if (Taikhoan != null)
                    {
                        return RedirectToAction("ResetPassword", "Account", new { username = model.Username });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc số điện thoại không đúng.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Đã xảy ra lỗi khi tìm kiếm tài khoản.");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đặt lại mật khẩu.");
                }
            }

            return View(model);
        }

        public IActionResult ResetPassword(string username)
        {
            var model = new ViewModels.Customer.ResetPasswordViewModel { Username = username };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ViewModels.Customer.ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword == model.ConfirmPassword)
                {
                    var Taikhoan = await _context.Taikhoans.FirstOrDefaultAsync(a => a.Username == model.Username);
                    if (Taikhoan != null)
                    {
                        _logger.LogInformation("Tài khoản tìm thấy: {Username}", model.Username);
                        Taikhoan.Password = model.NewPassword;

                        try
                        {
                            _context.Taikhoans.Update(Taikhoan);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Cập nhật mật khẩu thành công cho: {Username}", model.Username);
                            ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
                            return RedirectToAction("Login", "Account");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi khi lưu mật khẩu mới cho: {Username}", model.Username);
                            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu mật khẩu mới.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Không tìm thấy tài khoản: {Username}", model.Username);
                        ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu và xác nhận mật khẩu không khớp.");
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomerSchema");
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
