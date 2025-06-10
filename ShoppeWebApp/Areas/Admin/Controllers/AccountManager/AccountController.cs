using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Admin;
using System.Linq;
using ShoppeWebApp.Models; 

//******************************************************
// Xóa tài khoản sau 30 ngày có thể khôi phục được: CHƯA LÀM (Bổ sung sau)
//******************************************************
namespace ShoppeWebApp.Areas.Admin.Controllers.AccountManager
{
    [Authorize("Admin")]
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public AccountController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? searchQuery, string? role, int page = 1, int pageSize = 10)
        {
            // Đảm bảo giá trị hợp lệ cho page và pageSize
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;
        
            // Khởi tạo truy vấn
            var query = _context.Nguoidungs.AsQueryable();
        
            // Tìm kiếm theo từ khóa (ID, tên người dùng, CCCD, Email, địa chỉ)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(user =>
                    user.IdNguoiDung.Contains(searchQuery) ||
                    user.HoVaTen.Contains(searchQuery) ||
                    user.Cccd.Contains(searchQuery) ||
                    user.Email.Contains(searchQuery) ||
                    user.DiaChi.Contains(searchQuery));
            }
        
            // Lọc theo vai trò
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(user =>
                    (role == "Khách hàng" && user.VaiTro == 1) ||
                    (role == "Chủ Shop" && user.VaiTro == 2) ||
                    (role == "Admin" && user.VaiTro == 0));
            }
        
            // Tổng số người dùng
            int totalUsers = query.Count();
        
            // Tính toán số trang
            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
        
            // Lấy danh sách người dùng cho trang hiện tại
            var usersOnPage = query
                .Select(user => new UserViewModel
                {
                    Id = user.IdNguoiDung,
                    Status = user.TrangThai,
                    Name = user.HoVaTen,
                    Email = user.Email,
                    Cccd = user.Cccd,
                    Address = user.DiaChi,
                    Role = user.VaiTro == 1 ? "Khách hàng" : user.VaiTro == 2 ? "Chủ Shop" : "Admin"
                })
                .OrderByDescending(user => user.Status) // Sắp xếp giảm dần theo Status (1 trước, 0 sau)
                .ThenBy(user => user.Role == "Khách hàng" ? 1 : user.Role == "Chủ Shop" ? 2 : 3) // Sắp xếp theo Role
                .ThenBy(user => user.Id) // Sắp xếp theo Id
                .ThenBy(user => user.Name) // Sắp xếp theo Name
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        
            // Truyền dữ liệu phân trang và tham số tìm kiếm vào ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchQuery"] = searchQuery;
            ViewData["Role"] = role;
        
            // Trả về View với danh sách người dùng
            return View(usersOnPage);
        }
    

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAccountViewModel model)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                return View(model);
            }
        
            // Kiểm tra email đã tồn tại (trong cùng vai trò)
            if (_context.Nguoidungs.Any(user => user.Email == model.Email && user.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }
        
            // Kiểm tra CCCD đã tồn tại (trong cùng vai trò)
            if (_context.Nguoidungs.Any(user => user.Cccd == model.Cccd && user.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Cccd), "CCCD đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }
        
            // Kiểm tra SĐT đã tồn tại (trong cùng vai trò)
            if (_context.Nguoidungs.Any(user => user.Sdt == model.Sdt && user.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }
        
            // Tạo ID người dùng mới bằng cách tìm ID lớn nhất hiện tại và tăng lên
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
                newId = (long.Parse(maxId) + 1).ToString("D10"); // Tăng ID lên 1 và định dạng thành 10 chữ số
            }
        
            // Tạo đối tượng người dùng mới
            var newUser = new Nguoidung
            {
                IdNguoiDung = newId,
                HoVaTen = model.Name ?? string.Empty,
                Email = model.Email ?? string.Empty,
                Cccd = model.Cccd ?? string.Empty,
                Sdt = model.Sdt ?? string.Empty,
                DiaChi = model.Address ?? "Chưa cập nhật",
                VaiTro = model.Role,
                TrangThai = 1,
                ThoiGianTao = DateTime.Now
            };
        
            // Tạo tài khoản mới
            var newAccount = new Taikhoan
            {
                IdNguoiDung = newId,
                Username = model.Username ?? string.Empty,
                Password = model.Password ?? string.Empty
            };
        
            try
            {
                // Lưu vào cơ sở dữ liệu
                _context.Nguoidungs.Add(newUser);
                _context.Taikhoans.Add(newAccount);
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo tài khoản: " + ex.Message);
                return View(model);
            }
        }
    
        [HttpGet]
        public IActionResult Edit(string id)
        {
            // Tìm người dùng theo ID
            var user = _context.Nguoidungs.FirstOrDefault(u => u.IdNguoiDung == id);
            if (user == null)
            {
                return NotFound();
            }
        
            // Tìm tài khoản liên kết với người dùng
            var account = _context.Taikhoans.FirstOrDefault(a => a.IdNguoiDung == id);
            if (account == null)
            {
                return NotFound();
            }
        
            // Tạo ViewModel với dữ liệu từ cơ sở dữ liệu
            var model = new EditAccountViewModel
            {
                Id = user.IdNguoiDung,
                Username = account.Username,
                AvatarUrl = String.IsNullOrEmpty(user.UrlAnh)? "/Images/avatar-mac-dinh.jpg": $"/{user.UrlAnh}", // Đường dẫn ảnh đại diện
                Name = user.HoVaTen,
                Email = user.Email,
                Cccd = user.Cccd,
                Sdt = user.Sdt,
                Address = user.DiaChi,
                Role = user.VaiTro,
                Password = account.Password, // Mật khẩu hiện tại
                ConfirmPassword = account.Password // Xác nhận mật khẩu
            };
        
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
        
            // Tìm người dùng hiện tại theo ID
            var user = _context.Nguoidungs.FirstOrDefault(u => u.IdNguoiDung == model.Id);
            if (user == null)
            {
                return NotFound();
            }
        
            // Tìm tài khoản liên kết với người dùng
            var account = _context.Taikhoans.FirstOrDefault(a => a.IdNguoiDung == model.Id);
            if (account == null)
            {
                return NotFound();
            }
        
            // Kiểm tra email đã tồn tại (trừ người dùng hiện tại và cùng vai trò)
            if (_context.Nguoidungs.Any(u => u.Email == model.Email && u.IdNguoiDung != model.Id && u.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }
            
            // Kiểm tra CCCD đã tồn tại (trừ người dùng hiện tại và cùng vai trò)
            if (_context.Nguoidungs.Any(u => u.Cccd == model.Cccd && u.IdNguoiDung != model.Id && u.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Cccd), "CCCD đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }
            
            // Kiểm tra SĐT đã tồn tại (trừ người dùng hiện tại và cùng vai trò)
            if (_context.Nguoidungs.Any(u => u.Sdt == model.Sdt && u.IdNguoiDung != model.Id && u.VaiTro == model.Role))
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại đã được sử dụng bởi người dùng khác trong cùng vai trò.");
                return View(model);
            }

            // Kiểm tra nếu vai trò hiện tại là "Chủ Shop" và muốn nâng lên "Admin"
            if (user.VaiTro == 2 && model.Role == 0)
            {
                ModelState.AddModelError("", "Không được phép nâng vai trò từ Chủ Shop lên Admin.");
                return View(model);
            }

            // Kiểm tra nếu vai trò hiện tại là "Khách hàng" và muốn nâng lên "Chủ Shop" hoặc "Admin"
            if (user.VaiTro == 1 && (model.Role == 2 || model.Role == 0))
            {
                // Lấy danh sách đơn hàng của người dùng thông qua IdLienHe
                var pendingOrders = _context.Donhangs
                    .Where(dh => dh.IdLienHeNavigation.IdNguoiDung == user.IdNguoiDung && dh.TrangThai != 3) // Trạng thái khác 3 (chưa hoàn thành)
                    .ToList();
            
                // Nếu có bất kỳ đơn hàng nào chưa hoàn thành, không cho phép nâng cấp vai trò
                if (pendingOrders.Any())
                {
                    ModelState.AddModelError("", "Người dùng phải hoàn thành tất cả đơn hàng trước khi nâng cấp vai trò.");
                    return View(model);
                }
            }
        
            // Cập nhật thông tin người dùng
            model.AvatarUrl = "/Images/avatar-mac-dinh.jpg"; 
            user.HoVaTen = model.Name ?? user.HoVaTen;
            user.Email = model.Email ?? user.Email;
            user.Cccd = model.Cccd ?? user.Cccd;
            user.Sdt = model.Sdt ?? user.Sdt;
            user.DiaChi = model.Address ?? user.DiaChi;
            user.VaiTro = model.Role;
        
            // Cập nhật thông tin tài khoản
            account.Username = model.Username ?? account.Username;
        
            // Nếu mật khẩu được thay đổi, cập nhật mật khẩu
            if (!string.IsNullOrEmpty(model.Password))
            {
                account.Password = model.Password;
            }
        
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                return RedirectToAction("Edit", new { id = model.Id }); // Chuyển hướng lại trang chỉnh sửa
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật tài khoản: " + ex.Message);
                return View(model);
            }
        }  

        [HttpGet]
        public IActionResult Details(string id)
        {
            // Tìm người dùng theo ID
            var user = _context.Nguoidungs.FirstOrDefault(u => u.IdNguoiDung == id);
            if (user == null)
            {
                return NotFound();
            }
        
            // Tìm tài khoản liên kết với người dùng
            var account = _context.Taikhoans.FirstOrDefault(a => a.IdNguoiDung == id);
            if (account == null)
            {
                return NotFound();
            }
        
            // Tạo ViewModel với dữ liệu từ cơ sở dữ liệu
            var model = new EditAccountViewModel
            {
                Id = user.IdNguoiDung,
                Username = account.Username,
                AvatarUrl =  "/Images/avatar-mac-dinh.jpg", // Đường dẫn ảnh đại diện
                Name = user.HoVaTen,
                Email = user.Email,
                Cccd = user.Cccd,
                Sdt = user.Sdt,
                Address = user.DiaChi,
                Role = user.VaiTro,
                Password = account.Password, // Mật khẩu hiện tại
                ConfirmPassword = account.Password // Xác nhận mật khẩu
            };
        
            return View(model);
        }
   
         [HttpGet]
        public IActionResult Delete(string id)
        {
            // Tìm người dùng theo ID
            var user = _context.Nguoidungs.FirstOrDefault(u => u.IdNguoiDung == id && u.TrangThai == 1); // Chỉ lấy tài khoản đang hoạt động
            if (user == null)
            {
                return NotFound();
            }
        
            string warningMessage = string.Empty;
        
            if (user.VaiTro == 1) // Nếu là khách hàng
            {
                // Kiểm tra nếu người dùng là khách hàng và có đơn hàng
                var orders = _context.Donhangs
                    .Where(dh => dh.IdLienHeNavigation.IdNguoiDung == id)
                    .ToList();
        
                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        switch (order.TrangThai)
                        {
                            case 0: // Đơn hàng đang chờ xác nhận thanh toán
                                warningMessage = "Khách hàng đang có đơn hàng chờ xác nhận thanh toán, không thể xóa tài khoản.";
                                TempData["ErrorMessage"] = warningMessage;
                                return RedirectToAction("Index");
                            case 1: // Đơn hàng đã được xác nhận thanh toán
                                warningMessage = "Khách hàng có đơn hàng đã được xác nhận thanh toán. Nếu xóa, hệ thống sẽ tự động hoàn tiền.";
                                break;
                            default: // Các trạng thái khác
                                warningMessage = "Khách hàng hiện tại đang có đơn hàng.";
                                break;
                        }
                    }
                }
            }
            else if (user.VaiTro == 2) // Nếu là Chủ Shop
            {
                // Lấy danh sách sản phẩm thuộc cửa hàng của shop
                var shopProducts = _context.Sanphams
                    .Where(sp => sp.IdCuaHangNavigation.IdNguoiDung == id)
                    .Select(sp => sp.IdSanPham)
                    .ToList();
        
                // Kiểm tra nếu có sản phẩm nào thuộc đơn hàng
                var relatedOrders = _context.Chitietdonhangs
                    .Where(ctdh => shopProducts.Contains(ctdh.IdSanPham))
                    .Select(ctdh => ctdh.IdDonHang)
                    .Distinct()
                    .ToList();
        
                if (relatedOrders.Any())
                {
                    warningMessage = "Shop hiện tại đang có đơn hàng liên quan đến sản phẩm của cửa hàng. Bạn chắc chắn vẫn muốn xóa chứ?";
                }
            }
        
            // Tạo ViewModel để hiển thị thông tin người dùng
            var viewModel = new UserViewModel
            {
                Id = user.IdNguoiDung,
                Name = user.HoVaTen,
                Email = user.Email,
                Role = user.VaiTro == 1 ? "Khách hàng" : user.VaiTro == 2 ? "Chủ Shop" : "Admin",
                PhoneNumber = user.Sdt,
                Address = user.DiaChi,
                Cccd = user.Cccd,
                AvatarUrl = "/Images/avatar-mac-dinh.jpg", // Đường dẫn ảnh đại diện
            };
        
            TempData["WarningMessage"] = warningMessage; // Gửi cảnh báo đến View
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            // Tìm người dùng theo ID
            var user = _context.Nguoidungs.FirstOrDefault(u => u.IdNguoiDung == id && u.TrangThai == 1); // Chỉ xóa tài khoản đang hoạt động
            if (user == null)
            {
                return NotFound();
            }
        
            // Xóa mềm tài khoản
            user.TrangThai = 0; // Đánh dấu tài khoản là đã xóa
            user.ThoiGianXoa = DateTime.Now; // Ghi lại thời gian xóa
        
            try
            {
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tài khoản đã được xóa thành công!";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa tài khoản: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}