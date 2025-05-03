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
                    ThoiGianXoa = c.ThoiGianXoa,
                })
                .FirstOrDefault();
        
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Cửa hàng không tồn tại.";
                return RedirectToAction("Index", "Dashboard");
            }
        
            return View(shop);
        }

        public IActionResult Restore()
        {
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
        
            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }
        
            var cuaHang = _context.Cuahangs.FirstOrDefault(c => c.IdCuaHang == shopId);
            if (cuaHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }
        
            if (cuaHang.ThoiGianXoa.HasValue && (DateTime.Now - cuaHang.ThoiGianXoa.Value).TotalDays > 30)
            {
                TempData["ErrorMessage"] = "Cửa hàng đã bị xóa quá 30 ngày và không thể khôi phục.";
                return RedirectToAction("Index");
            }
        
            cuaHang.ThoiGianXoa = null;
            cuaHang.TrangThai = 1; 
            _context.SaveChanges();
        
            TempData["SuccessMessage"] = "Cửa hàng đã được khôi phục thành công.";
            return RedirectToAction("Index");
        }  

        [HttpGet]
        public IActionResult Edit()
        {
            // Lấy IdCuaHang từ Claim
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;

            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin cửa hàng từ cơ sở dữ liệu
            var shop = _context.Cuahangs
                .Where(c => c.IdCuaHang == shopId)
                .Select(c => new EditShopViewModel
                {
                    IdCuaHang = c.IdCuaHang,
                    TenCuaHang = c.TenCuaHang,
                    Sdt = c.Sdt,
                    DiaChi = c.DiaChi,
                    MoTa = c.MoTa,
                    UrlAnhHienTai = c.UrlAnh
                })
                .FirstOrDefault();

            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }

            return View(shop);
        }

        [HttpPost]
        public IActionResult Edit(EditShopViewModel model)
        {
            // Lấy IdCuaHang từ Claim
            var shopId = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;

            if (string.IsNullOrEmpty(shopId))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy cửa hàng từ cơ sở dữ liệu
            var shop = _context.Cuahangs.FirstOrDefault(c => c.IdCuaHang == shopId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy cửa hàng.";
                return RedirectToAction("Index");
            }


            // Cập nhật thông tin cửa hàng
            shop.TenCuaHang = model.TenCuaHang;
            shop.Sdt = model.Sdt;
            shop.DiaChi = model.DiaChi;
            shop.MoTa = model.MoTa;

            // Xử lý ảnh mới nếu có
            if (model.UrlAnhMoi != null)
            {
                // Tạo tên file duy nhất bằng UUID
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.UrlAnhMoi.FileName)}";
            
                // Đường dẫn lưu file
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Shop");
                var uploadPath = Path.Combine(uploadFolder, uniqueFileName);
            
                // Kiểm tra và tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
            
                // Lưu file vào thư mục
                using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                {
                    model.UrlAnhMoi.CopyTo(fileStream);
                }
            
                // Cập nhật URL ảnh mới
                shop.UrlAnh = $"/images/Shop/{uniqueFileName}";
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Cuahangs.Update(shop);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin cửa hàng thành công!";
            return RedirectToAction("Index");
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