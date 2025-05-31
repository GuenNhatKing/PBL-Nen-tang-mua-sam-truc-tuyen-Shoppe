using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels;
using ShoppeWebApp.Services;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class AccountController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ShoppeWebAppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(ViewModels.Seller.LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var potentialAccounts = _context.Taikhoans
                    .Include(a => a.IdNguoiDungNavigation)
                    .Where(a => a.Username.ToLower() == model.Username.ToLower() && a.Password == PasswordHasher.ComputeSha256Hash(model.Password))
                    .AsEnumerable();
                
                var account = potentialAccounts.FirstOrDefault(a => a.Username == model.Username);
        
                if (account != null)
                {
                    // Kiểm tra vai trò người dùng
                    if (account.IdNguoiDungNavigation.VaiTro == Constants.SELLER_ROLE)
                    {
                        Console.WriteLine($"Đăng nhập cho seller, id={account.IdNguoiDung}");
        
                        // Kiểm tra xem người dùng có cửa hàng hay không
                        var shop = _context.Cuahangs.FirstOrDefault(c => c.IdNguoiDung == account.IdNguoiDung);
                        var shopId = _context.Cuahangs
                            .Where(c => c.IdNguoiDung == account.IdNguoiDung)
                            .Select(c => c.IdCuaHang)
                            .FirstOrDefault();
                        if (shopId == null)
                        {
                            TempData["ErrorMessage"] = "Bạn chưa có cửa hàng. Vui lòng tạo cửa hàng mới.";
                            return RedirectToAction("Create", "Shop");
                        }
        
                        // Tạo Claims để lưu thông tin đăng nhập
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim("IdNguoiDung", account.IdNguoiDung),
                            new Claim("IdCuaHang", shop.IdCuaHang), // Lưu IdCuaHang vào Claims
                            new Claim("TenCuaHang", shop.TenCuaHang), // Lưu tên cửa hàng vào Claims
                            new Claim(ClaimTypes.Name, account.Username),
                            new Claim(ClaimTypes.Role, "Seller")
                        }, "SellerSchema");
                        
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(Constants.COOKIE_EXPIRY_DAYS)
                        };
                    
                        // Đăng nhập
                        await HttpContext.SignInAsync("SellerSchema", principal, properties);
        
                        // Chuyển hướng đến Dashboard
                        return RedirectToAction("Index", "Dashboard", new { area = "Seller" });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản không có quyền truy cập vào kênh người bán.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
        
            return View(model);
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ViewModels.Seller.ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = _context.Taikhoans
                        .FirstOrDefault(a => a.Username == model.Username && a.IdNguoiDungNavigation.Sdt == model.PhoneNumber);

                    if (account != null)
                    {
                        return RedirectToAction("ResetPassword", "Account", new {username = model.Username });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc số điện thoại không đúng.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while resetting the password.");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đặt lại mật khẩu.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string username)
        {
            var model = new ViewModels.Seller.ResetPasswordViewModel { Username = username };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ViewModels.Seller.ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the new password and confirm password fields match
                if (model.NewPassword == model.ConfirmPassword)
                {
                    var account = await _context.Taikhoans.FirstOrDefaultAsync(a => a.Username == model.Username);
                    if (account != null)
                    {
                        account.Password = PasswordHasher.ComputeSha256Hash(model.NewPassword);
                        try
                        {
                            await _context.SaveChangesAsync();
                            ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
                            return RedirectToAction("Login", "Account");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while saving the new password.");
                            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu mật khẩu mới.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu và xác nhận mật khẩu không khớp.");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error in {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(ViewModels.Seller.RegisterViewModel model)
        {
            // Kiểm tra tất cả các trường không được để trống
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            // Kiểm tra CCCD phải đủ 12 số
            if (model.Cccd.Length != 12 || !model.Cccd.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(model.Cccd), "CCCD phải là 12 chữ số.");
                return View(model);
            }

            // Tạo ID cho người bán
            var maxId = _context.Nguoidungs
                .OrderByDescending(u => u.IdNguoiDung)
                .Select(u => u.IdNguoiDung)
                .FirstOrDefault();

            string newId = string.IsNullOrEmpty(maxId) ? "0000000001" : (long.Parse(maxId) + 1).ToString("D10");

            var Nguoidung = new Nguoidung
            {
                IdNguoiDung = newId,
                HoVaTen = model.HoVaTen,
                Email = model.Email,
                Sdt = model.Sdt,
                Cccd = model.Cccd,
                DiaChi = "Chưa cập nhật",
                VaiTro = Constants.SELLER_ROLE,
                TrangThai = 1,
                ThoiGianTao = DateTime.Now
            };

            var Taikhoan = new Taikhoan
            {
                Username = model.TenDangNhap,
                Password = PasswordHasher.ComputeSha256Hash(model.MatKhau),
                IdNguoiDung = newId
            };

            // Tạo ID cho cửa hàng
            var maxShopId = _context.Cuahangs
                .OrderByDescending(shop => shop.IdCuaHang)
                .Select(shop => shop.IdCuaHang)
                .FirstOrDefault();

            string newShopId = string.IsNullOrEmpty(maxShopId) ? "0000000001" : (long.Parse(maxShopId) + 1).ToString("D10");

            var newShop = new Cuahang
            {
                IdCuaHang = newShopId,
                IdNguoiDung = newId,
                TenCuaHang = "Cửa hàng của " + model.HoVaTen, 
                DiaChi = "Chưa cập nhật", 
                MoTa = "Chưa cập nhật", 
                Sdt = model.Sdt, 
                UrlAnh = "/images/shop-dai-dien.png", 
                ThoiGianTao = DateTime.Now,
                TrangThai = 1
            };

            try
            {
                _context.Nguoidungs.Add(Nguoidung);
                _context.Taikhoans.Add(Taikhoan);
                _context.Cuahangs.Add(newShop);
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
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("SellerSchema");
            return RedirectToAction("Login", "Account", new { area = "Seller" });
        }
    }
}