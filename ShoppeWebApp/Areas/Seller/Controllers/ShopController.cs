using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Seller;
using System.Linq;
using ShoppeWebApp.Data;
using System.Security.Claims;

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Authorize(AuthenticationSchemes = "SellerSchema", Roles = "Seller")]
    [Area("Seller")]
    public class ShopController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public ShopController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
        
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }
        
            var shop = _context.Cuahangs
                .Where(c => c.IdCuaHang == shopId)
                .Select(c => new ShopViewModel
                {
                    IdCuaHang = c.IdCuaHang,
                    TenCuaHang = c.TenCuaHang,
                    IdSeller = c.IdNguoiDung,
                    DiaChi = c.DiaChi,
                    MoTa = c.MoTa,
                    Sdt = c.Sdt,
                    UrlAnh = c.UrlAnh,
                    ThoiGianTao = c.ThoiGianTao,
                })
                .FirstOrDefault();
        
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Cửa hàng không tồn tại.";
                return RedirectToAction("Index", "Dashboard");
            }
        
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
            var shop = _context.Cuahangs
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
            var shop = _context.Cuahangs.FirstOrDefault(s => s.IdCuaHang == model.IdCuaHang);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Kiểm tra tên cửa hàng không trùng với cửa hàng khác
            if (_context.Cuahangs.Any(s => s.TenCuaHang == model.TenCuaHang && s.IdCuaHang != model.IdCuaHang))
            {
                ModelState.AddModelError(nameof(model.TenCuaHang), "Tên cửa hàng đã tồn tại.");
                return View(model);
            }
        
            // Kiểm tra số điện thoại không trùng với cửa hàng khác
            if (_context.Cuahangs.Any(s => s.Sdt == model.Sdt && s.IdCuaHang != model.IdCuaHang))
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
                _context.Cuahangs.Update(shop);
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Cập nhật thông tin cửa hàng thành công!";
                return RedirectToAction("Index");
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
            Console.WriteLine("Delete method called with id: " + id);
            // Kiểm tra nếu ID cửa hàng không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID cửa hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
        
            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.Cuahangs
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
                    SoSanPham = s.Sanphams.Count(),
                    SoDonHang = _context.Donhangs
                        .Where(dh => dh.Chitietdonhangs
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
            var shop = _context.Cuahangs.FirstOrDefault(s => s.IdCuaHang == id);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            // Kiểm tra nếu cửa hàng có đơn hàng liên quan
            var hasOrders = _context.Donhangs
                .Any(dh => dh.Chitietdonhangs
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
                _context.Cuahangs.Update(shop);
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