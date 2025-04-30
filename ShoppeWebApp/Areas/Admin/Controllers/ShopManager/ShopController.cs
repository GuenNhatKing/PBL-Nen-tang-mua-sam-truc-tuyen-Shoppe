using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Admin;
using System.Linq;
using ShoppeWebApp.Data; 

namespace ShoppeWebApp.Areas.Admin.Controllers.ShopManager
{
    [Authorize("Admin")]
    [Area("Admin")]
    public class ShopController : Controller
    {
        private readonly ShoppeWebAppContext _context;

        public ShopController(ShoppeWebAppContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchQuery, int page = 1, int pageSize = 12)
        {
            // Đảm bảo giá trị hợp lệ cho page và pageSize
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 12 : pageSize;
        
            // Khởi tạo truy vấn
            var query = _context.CuaHangs.AsQueryable();
        
            // Tìm kiếm theo ID cửa hàng, ID chủ sở hữu hoặc tên cửa hàng
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(shop =>
                    shop.IdCuaHang.Contains(searchQuery) ||
                    shop.TenCuaHang.Contains(searchQuery) ||
                    shop.IdNguoiDung.Contains(searchQuery) ||
                    shop.IdNguoiDungNavigation.HoVaTen.Contains(searchQuery) ||
                    shop.Sdt.Contains(searchQuery));
            }
        
            // Tổng số cửa hàng
            int totalShops = query.Count();
        
            // Tính toán số trang
            int totalPages = (int)Math.Ceiling((double)totalShops / pageSize);
        
            // Lấy danh sách cửa hàng cho trang hiện tại
            var shopsOnPage = query
                .Select(shop => new ShopViewModel
                {
                    IdCuaHang = shop.IdCuaHang,
                    TenCuaHang = shop.TenCuaHang,
                    IdSeller = shop.IdNguoiDung,
                    TenSeller = shop.IdNguoiDungNavigation.HoVaTen,
                    Sdt = shop.Sdt,
                    UrlAnh = shop.UrlAnh,
                    ThoiGianTao = shop.ThoiGianTao
                })
                .OrderBy(shop => shop.TenCuaHang)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        
            // Truyền dữ liệu phân trang và tham số tìm kiếm vào ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchQuery"] = searchQuery;
        
            // Trả về View với danh sách cửa hàng
            return View(shopsOnPage);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateShopViewModel model)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                return View(model);
            }
        
            string filePath = string.Empty;
        
            if (model.UrlAnh != null && model.UrlAnh.Length > 0)
            {
                // Đường dẫn lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Shop");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
        
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UrlAnh.FileName);
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
        
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.UrlAnh.CopyTo(fileStream);
                }
        
                // Lưu đường dẫn tệp vào cơ sở dữ liệu
                filePath = "/Images/Shop/" + uniqueFileName;
            }
            else
            {
                // Nếu không tải lên ảnh, sử dụng ảnh mặc định
                filePath = "/Images/shop-dai-dien.png";
            }
        
            Console.WriteLine("File path: " + filePath);
        
            // Kiểm tra vai trò của chủ Shop
            var seller = _context.NguoiDungs.FirstOrDefault(user => user.IdNguoiDung == model.IdSeller);
            if (seller == null || seller.VaiTro != 2)
            {
                ModelState.AddModelError(nameof(model.IdSeller), "Chủ sở hữu phải có vai trò là Chủ Shop.");
                return View(model);
            }
        
            // Kiểm tra nếu chủ Shop đã có cửa hàng
            if (_context.CuaHangs.Any(shop => shop.IdNguoiDung == model.IdSeller))
            {
                ModelState.AddModelError(nameof(model.IdSeller), "Chủ Shop này đã sở hữu một cửa hàng.");
                return View(model);
            }
        
            // Kiểm tra tên cửa hàng đã tồn tại
            if (_context.CuaHangs.Any(shop => shop.TenCuaHang == model.TenCuaHang))
            {
                ModelState.AddModelError(nameof(model.TenCuaHang), "Tên cửa hàng đã tồn tại.");
                return View(model);
            }
        
            // Kiểm tra số điện thoại đã tồn tại
            if (_context.CuaHangs.Any(shop => shop.Sdt == model.Sdt))
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại đã được sử dụng bởi cửa hàng khác.");
                return View(model);
            }
        
            // Tạo ID cửa hàng mới bằng cách tìm ID lớn nhất hiện tại và tăng lên
            var maxId = _context.CuaHangs
                .OrderByDescending(shop => shop.IdCuaHang)
                .Select(shop => shop.IdCuaHang)
                .FirstOrDefault();
        
            string newId;
            if (string.IsNullOrEmpty(maxId))
            {
                newId = "0000000001"; // Nếu chưa có ID nào, bắt đầu từ 0000000001
            }
            else
            {
                // Tăng ID lên 1 và định dạng thành 10 chữ số
                newId = (long.Parse(maxId) + 1).ToString("D10");
            }
        
            // Tạo đối tượng cửa hàng mới
            var newShop = new CuaHang
            {
                IdCuaHang = newId,
                TenCuaHang = model.TenCuaHang,
                IdNguoiDung = model.IdSeller,
                Sdt = model.Sdt,
                DiaChi = model.DiaChi,
                MoTa = model.MoTa ?? "Chưa cập nhật",
                UrlAnh = filePath ?? "/Images/shop-dai-dien.png",
                ThoiGianTao = DateTime.Now,
                TrangThai = 1
            };
        
            try
            {
                // Lưu vào cơ sở dữ liệu
                _context.CuaHangs.Add(newShop);
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Tạo cửa hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo cửa hàng: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DetailsShop(string id)
        {
            // Kiểm tra nếu ID cửa hàng không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID cửa hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.CuaHangs
                .Where(s => s.IdCuaHang == id)
                .Select(s => new ShopViewModel
                {
                    IdCuaHang = s.IdCuaHang,
                    TenCuaHang = s.TenCuaHang,
                    IdSeller = s.IdNguoiDung,
                    TenSeller = s.IdNguoiDungNavigation.HoVaTen, // Giả sử bạn có liên kết đến bảng người dùng
                    Sdt = s.Sdt,
                    UrlAnh = s.UrlAnh,
                    DiaChi = s.DiaChi,
                    MoTa = s.MoTa,
                    ThoiGianTao = s.ThoiGianTao,
                    SoSanPham = s.SanPhams.Count, // Đếm số sản phẩm thuộc cửa hàng
                    SoDonHang = _context.DonHangs // Đếm số đơn hàng liên quan đến sản phẩm của cửa hàng
                        .Where(dh => dh.ChiTietDonHangs
                            .Any(ct => ct.IdSanPhamNavigation.IdCuaHang == id))
                        .Count()
                })
                .FirstOrDefault();
        
            // Kiểm tra nếu không tìm thấy cửa hàng
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Truyền dữ liệu vào View
            return View(shop);
        }
    
        [HttpGet]
        public IActionResult Edit(string id)
        {
            // Kiểm tra nếu ID cửa hàng không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID cửa hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.CuaHangs
                .Where(s => s.IdCuaHang == id)
                .Select(s => new EditShopViewModel
                {
                    IdCuaHang = s.IdCuaHang,
                    TenCuaHang = s.TenCuaHang,
                    IdSeller = s.IdNguoiDung,
                    Sdt = s.Sdt,
                    DiaChi = s.DiaChi,
                    MoTa = s.MoTa,
                    UrlAnhHienTai = s.UrlAnh
                })
                .FirstOrDefault();
        
            // Kiểm tra nếu không tìm thấy cửa hàng
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Truyền dữ liệu vào View
            return View(shop);
        }
        
         [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditShopViewModel model)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                return View(model);
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.CuaHangs.FirstOrDefault(s => s.IdCuaHang == model.IdCuaHang);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Kiểm tra tên cửa hàng không trùng với cửa hàng khác
            if (_context.CuaHangs.Any(s => s.TenCuaHang == model.TenCuaHang && s.IdCuaHang != model.IdCuaHang))
            {
                ModelState.AddModelError(nameof(model.TenCuaHang), "Tên cửa hàng đã tồn tại.");
                return View(model);
            }
        
            // Kiểm tra số điện thoại không trùng với cửa hàng khác
            if (_context.CuaHangs.Any(s => s.Sdt == model.Sdt && s.IdCuaHang != model.IdCuaHang))
            {
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại đã được sử dụng bởi cửa hàng khác.");
                return View(model);
            }
        
            // Cập nhật thông tin cửa hàng
            shop.TenCuaHang = model.TenCuaHang;
            shop.Sdt = model.Sdt;
            shop.DiaChi = model.DiaChi;
            shop.MoTa = model.MoTa;
        
            // Xử lý ảnh mới nếu có
            if (model.UrlAnhMoi != null && model.UrlAnhMoi.Length > 0)
            {
                // Đường dẫn lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Shop");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
        
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UrlAnhMoi.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.UrlAnhMoi.CopyTo(fileStream);
                }
        
                // Xóa ảnh cũ nếu không phải ảnh mặc định
                if (!string.IsNullOrEmpty(shop.UrlAnh) && shop.UrlAnh != "/Images/shop-dai-dien.png")
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", shop.UrlAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
        
                // Lưu đường dẫn ảnh mới vào cơ sở dữ liệu
                shop.UrlAnh = "/Images/Shop/" + uniqueFileName;
            }
        
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.CuaHangs.Update(shop);
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Cập nhật thông tin cửa hàng thành công!";
                return RedirectToAction("Edit");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật cửa hàng: " + ex.Message);
                return View(model);
            }
        }
        
        [HttpGet]
        public IActionResult Delete(string id)
        {
            // Kiểm tra nếu ID cửa hàng không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID cửa hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.CuaHangs
                .Where(s => s.IdCuaHang == id)
                .Select(s => new ShopViewModel
                {
                    IdCuaHang = s.IdCuaHang,
                    TenCuaHang = s.TenCuaHang,
                    IdSeller = s.IdNguoiDung,
                    TenSeller = s.IdNguoiDungNavigation.HoVaTen,
                    Sdt = s.Sdt,
                    UrlAnh = s.UrlAnh,
                    DiaChi = s.DiaChi,
                    MoTa = s.MoTa,
                    ThoiGianTao = s.ThoiGianTao,
                    SoSanPham = s.SanPhams.Count,
                    SoDonHang = _context.DonHangs
                        .Where(dh => dh.ChiTietDonHangs
                            .Any(ct => ct.IdSanPhamNavigation.IdCuaHang == id))
                        .Count()
                })
                .FirstOrDefault();
        
            // Kiểm tra nếu không tìm thấy cửa hàng
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Kiểm tra nếu cửa hàng có sản phẩm hoặc đơn hàng liên quan
            if (shop.SoSanPham > 0 || shop.SoDonHang > 0)
            {
                TempData["WarningMessage"] = "Cửa hàng hiện tại đang có sản phẩm hoặc đơn hàng liên quan. Bạn chắc chắn muốn xóa?";
            }
        
            // Truyền dữ liệu vào View
            return View(shop);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            // Kiểm tra nếu ID cửa hàng không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID cửa hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.CuaHangs.FirstOrDefault(s => s.IdCuaHang == id);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Kiểm tra nếu cửa hàng có đơn hàng liên quan
            var hasOrders = _context.DonHangs
                .Any(dh => dh.ChiTietDonHangs
                    .Any(ct => ct.IdSanPhamNavigation.IdCuaHang == id));
        
            if (hasOrders)
            {
                TempData["WarningMessage"] = "Cửa hàng hiện tại đang có đơn hàng liên quan. Hệ thống vẫn sẽ xóa cửa hàng.";
            }
        
            try
            {
                // Xóa mềm cửa hàng (đánh dấu trạng thái là đã xóa)
                shop.TrangThai = 0; // Đánh dấu cửa hàng là đã xóa
                shop.ThoiGianXoa = DateTime.Now; // Ghi lại thời gian xóa
                _context.CuaHangs.Update(shop);
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Cửa hàng đã được xóa thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa cửa hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}