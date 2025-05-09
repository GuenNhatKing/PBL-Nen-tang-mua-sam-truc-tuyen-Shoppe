using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.ViewModels.Seller;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;  

namespace ShoppeWebApp.Areas.Seller.Controllers
{
    [Authorize(AuthenticationSchemes = "SellerSchema", Roles = "Seller")]
    [Area("Seller")]
    public class ProductController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public ProductController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string IdDanhMuc, string searchQuery, int page = 1, int pageSize = 12)
        {
            // Lấy IdCuaHang từ Claim
            var idCuaHang = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
        
            if (string.IsNullOrEmpty(idCuaHang))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }
        
            // Lấy thông tin cửa hàng
            var cuaHang = _context.Cuahangs.FirstOrDefault(ch => ch.IdCuaHang == idCuaHang);
            if (cuaHang == null)
            {
                return NotFound("Không tìm thấy cửa hàng.");
            }
        
            // Lấy danh sách danh mục thuộc cửa hàng
            var categories = _context.Sanphams
                .Where(p => p.IdCuaHang == idCuaHang && !string.IsNullOrEmpty(p.IdDanhMuc))
                .Select(p => p.IdDanhMuc)
                .Distinct()
                .Join(_context.Danhmucs, id => id, c => c.IdDanhMuc, (id, c) => new CategoryInfo
                {
                    IdDanhMuc = c.IdDanhMuc,
                    TenDanhMuc = c.TenDanhMuc
                })
                .ToList();
        
            // Kiểm tra nếu không có danh mục
            if (!categories.Any())
            {
                var emptyViewModel = new ProductViewModel
                {
                    Products = new List<ProductInfo>(),
                    Categories = new List<CategoryInfo>(),
                    IdCuaHang = idCuaHang,
                    TenCuaHang = cuaHang.TenCuaHang,
                    DiaChi = cuaHang.DiaChi,
                    SoDienThoai = cuaHang.Sdt,
                    UrlAnhCuaHang = cuaHang.UrlAnh
                };
                return View(emptyViewModel);
            }
        
            // Lọc sản phẩm theo danh mục, tìm kiếm và cửa hàng
            var query = _context.Sanphams.Where(p => p.IdCuaHang == idCuaHang).AsQueryable();
            if (!string.IsNullOrEmpty(IdDanhMuc))
            {
                query = query.Where(p => p.IdDanhMuc == IdDanhMuc);
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.TenSanPham.Contains(searchQuery));
            }
        
            // Phân trang
            int totalProducts = query.Count();
            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductInfo
                {
                    IdSanPham = p.IdSanPham,
                    TenSanPham = p.TenSanPham,
                    GiaGoc = p.GiaGoc,
                    GiaBan = p.GiaBan,
                    TyLeGiamGia = p.GiaGoc > 0 ? (int)((1 - (p.GiaBan / p.GiaGoc)) * 100) : 0,
                    SoLuongBan = p.SoLuongBan,
                    UrlAnh = p.UrlAnh
                })
                .ToList();
        
            // Tạo ViewModel
            var viewModel = new ProductViewModel
            {
                Products = products,
                Categories = categories,
                IdCuaHang = idCuaHang,
                TenCuaHang = cuaHang.TenCuaHang,
                DiaChi = cuaHang.DiaChi,
                SoDienThoai = cuaHang.Sdt,
                UrlAnhCuaHang = cuaHang.UrlAnh
            };
        
            // Truyền thông tin phân trang qua ViewData
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)System.Math.Ceiling((double)totalProducts / pageSize);
        
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(string IdCuaHang)
        {
            var model = new CreateProductViewModel
            {
                IdCuaHang = IdCuaHang
            };
            Console.WriteLine($"IdCuaHang: {IdCuaHang}");
        
            // Lấy danh sách danh mục để hiển thị trong dropdown
            var categories = _context.Danhmucs
                .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                .ToList();
        
            ViewBag.Categories = categories;
        
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy danh sách danh mục để hiển thị lại trong View
                ViewBag.Categories = _context.Danhmucs
                    .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                    .ToList();
        
                // Trả về View với lỗi
                return View(model);
            }
        
            // Tải ảnh lên nếu có
            string? imagePath = null;
            if (model.UrlAnh != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.UrlAnh.FileName}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.UrlAnh.CopyTo(stream);
                }
                imagePath = $"Images/Products/{fileName}";
            }
        
            // Tạo ID sản phẩm mới bằng cách tìm ID lớn nhất hiện tại và tăng lên
            var maxId = _context.Sanphams
                .OrderByDescending(p => p.IdSanPham)
                .Select(p => p.IdSanPham)
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
        
            // Tạo sản phẩm mới
            var product = new Sanpham
            {
                IdSanPham = newId,
                IdCuaHang = model.IdCuaHang,
                IdDanhMuc = model.IdDanhMuc,
                TenSanPham = model.TenSanPham,
                UrlAnh = imagePath,
                MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? "Chưa cập nhật" : model.MoTa,
                SoLuongKho = model.SoLuongKho ?? 0,
                GiaGoc = model.GiaGoc ?? 0,
                GiaBan = model.GiaBan ?? 0,
                TrangThai = 1, // Mặc định là hoạt động
                ThoiGianTao = DateTime.Now
            };
        
            _context.Sanphams.Add(product);
            _context.SaveChanges();
        
            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index", new { IdCuaHang = model.IdCuaHang });
        }

        [HttpGet]
        public IActionResult EditProduct(string id) // Changed parameter name from IdSanPham to id
        {
            // Log the request details for debugging
            Console.WriteLine($"Request path: {Request.Path}");
            Console.WriteLine($"Request query string: {Request.QueryString}");
            Console.WriteLine($"Route data: {string.Join(", ", RouteData.Values.Select(v => $"{v.Key}={v.Value}"))}");
            Console.WriteLine($"Initial id parameter value: '{id}'");

            // Use the parameter directly since it's now correctly bound
            string IdSanPham = id;
            
            // Your existing fallback logic in case the parameter is still empty
            if (string.IsNullOrEmpty(IdSanPham))
            {
                // Try to get from query string first
                IdSanPham = Request.Query["IdSanPham"].ToString();
                Console.WriteLine($"ID from query string: '{IdSanPham}'");
                
                // Then from route data
                if (string.IsNullOrEmpty(IdSanPham) && RouteData.Values.ContainsKey("id"))
                {
                    IdSanPham = RouteData.Values["id"]?.ToString();
                    Console.WriteLine($"ID from route data 'id': '{IdSanPham}'");
                }
            }

            // Ensure database context is available
            if (_context.Sanphams == null)
            {
                Console.WriteLine("Database context for Sanphams is null");
                return NotFound("Không thể kết nối đến cơ sở dữ liệu.");
            }

            // Try to find product with exact match first
            var product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == IdSanPham);
            
            // If not found, try with trimming and other variations
            if (product == null && !string.IsNullOrEmpty(IdSanPham))
            {
                // Try with trimming
                product = _context.Sanphams.FirstOrDefault(p => 
                    p.IdSanPham.Trim() == IdSanPham.Trim());
                
                // Try with numeric comparison if it's a number
                if (product == null && long.TryParse(IdSanPham, out long idAsNumber))
                {
                    // Format the ID as expected in the database (with leading zeros)
                    string formattedId = idAsNumber.ToString("D10");
                    product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == formattedId);
                    Console.WriteLine($"Trying formatted ID: '{formattedId}', found product: {product != null}");
                }
            }

            if (product == null)
            {
                Console.WriteLine($"Product with ID '{IdSanPham}' not found in database.");
                return NotFound("Không tìm thấy sản phẩm với ID này.");
            }

            var idCuaHang = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
            if (string.IsNullOrEmpty(idCuaHang))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Check if product belongs to the seller's shop
            if (product.IdCuaHang != idCuaHang)
            {
                TempData["ErrorMessage"] = "Sản phẩm này không thuộc cửa hàng của bạn.";
                return RedirectToAction("Index");
            }

            // Get category name for display
            var categoryName = _context.Danhmucs
                .Where(c => c.IdDanhMuc == product.IdDanhMuc)
                .Select(c => c.TenDanhMuc)
                .FirstOrDefault() ?? "Không có danh mục";

            // Create view model
            var model = new EditProductViewModel
            {
                IdSanPham = product.IdSanPham,
                IdCuaHang = product.IdCuaHang,
                IdDanhMuc = product.IdDanhMuc,
                TenDanhMuc = categoryName,
                TenSanPham = product.TenSanPham,
                MoTa = product.MoTa,
                SoLuongKho = product.SoLuongKho,
                GiaGoc = product.GiaGoc,
                GiaBan = product.GiaBan,
                UrlAnh = product.UrlAnh,
                SoLuongBan = product.SoLuongBan
            };

            // Load categories for dropdown
            ViewBag.Categories = _context.Danhmucs
                .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                .ToList();
                
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(EditProductViewModel model)
        {
            if (string.IsNullOrEmpty(model.IdSanPham) && RouteData.Values.ContainsKey("id"))
            {
                model.IdSanPham = RouteData.Values["id"]?.ToString();
                Console.WriteLine($"POST EditProduct - Using ID from route: '{model.IdSanPham}'");
            }

            if (string.IsNullOrEmpty(model.IdSanPham))
            {
                TempData["ErrorMessage"] = "Mã sản phẩm không hợp lệ hoặc không được cung cấp.";
                return RedirectToAction("Index");
            }

            var product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == model.IdSanPham);
            
            if (product == null && long.TryParse(model.IdSanPham, out long idAsNumber))
            {
                string formattedId = idAsNumber.ToString("D10");
                product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == formattedId);
                
                if (product != null)
                {
                    model.IdSanPham = formattedId;
                }
            }
            
            if (product == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy sản phẩm với mã: {model.IdSanPham}";
                return RedirectToAction("Index");
            }
            
            var idCuaHang = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;
            
            if (string.IsNullOrEmpty(idCuaHang) || product.IdCuaHang != idCuaHang)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa sản phẩm này.";
                return RedirectToAction("Index");
            }
            
            model.IdCuaHang = product.IdCuaHang;
            model.UrlAnh = product.UrlAnh; 
            model.TenDanhMuc = _context.Danhmucs
                .Where(c => c.IdDanhMuc == model.IdDanhMuc)
                .Select(c => c.TenDanhMuc)
                .FirstOrDefault() ?? "Không có danh mục";
                
            ModelState.Remove("IdCuaHang");
            ModelState.Remove("UrlAnh");
            ModelState.Remove("TenDanhMuc");
            ModelState.Remove("NewUrlAnh");
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                
                ViewBag.Categories = _context.Danhmucs
                    .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                    .ToList();
                
                model.UrlAnh = product.UrlAnh;
                
                return View(model);
            }

            var originalName = product.TenSanPham;
            var originalPrice = product.GiaBan;

            product.TenSanPham = model.TenSanPham;
            product.IdDanhMuc = model.IdDanhMuc;
            product.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? "Chưa cập nhật" : model.MoTa;
            product.SoLuongKho = model.SoLuongKho ?? 0;
            product.GiaGoc = model.GiaGoc ?? 0;
            product.GiaBan = model.GiaBan ?? 0;

            Console.WriteLine($"Updating product: {originalName} -> {product.TenSanPham}, Price: {originalPrice} -> {product.GiaBan}");

            product.ThoiGianTao = DateTime.Now;

            if (model.NewUrlAnh != null && model.NewUrlAnh.Length > 0)
            {
                Console.WriteLine("Uploading new image");
                if (!string.IsNullOrEmpty(product.UrlAnh) && 
                    !product.UrlAnh.Contains("default") && 
                    System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.UrlAnh)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.UrlAnh));
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.NewUrlAnh.FileName)}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.NewUrlAnh.CopyTo(stream);
                }
                product.UrlAnh = $"Images/Products/{fileName}";
            }
            else
            {
                Console.WriteLine("Keeping existing image: " + product.UrlAnh);
            }

            try
            {
                _context.Entry(product).State = EntityState.Modified;
                int rowsAffected = _context.SaveChanges();
                
                Console.WriteLine($"Product updated successfully. ID: {product.IdSanPham}, Rows affected: {rowsAffected}");
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("DetailsProduct", new { IdSanPham = product.IdSanPham });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                ViewBag.Categories = _context.Danhmucs
                    .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                    .ToList();
                model.UrlAnh = product.UrlAnh;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DetailsProduct(string IdSanPham)
        {
            var idCuaHang = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;

            if (string.IsNullOrEmpty(idCuaHang))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == IdSanPham && p.IdCuaHang == idCuaHang);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Lấy danh sách đánh giá cho sản phẩm
            var danhGias = _context.Danhgia
                .Where(dg => dg.IdSanPham == IdSanPham)
                .Select(dg => new DetailsProductViewModel.DanhGiaInfo
                {
                    IdDanhGia = dg.IdDanhGia,
                    IdNguoiDung = dg.IdNguoiDung,
                    TenNguoiDung = _context.Nguoidungs
                        .Where(nd => nd.IdNguoiDung == dg.IdNguoiDung)
                        .Select(nd => nd.HoVaTen)
                        .FirstOrDefault(),
                    DiemDanhGia = dg.DiemDanhGia,
                    NoiDung = dg.NoiDung,
                    ThoiGianDG = dg.ThoiGianDg
                })
                .ToList();

            var viewModel = new DetailsProductViewModel
            {
                IdSanPham = product.IdSanPham,
                TenSanPham = product.TenSanPham,
                TenDanhMuc = _context.Danhmucs.FirstOrDefault(dm => dm.IdDanhMuc == product.IdDanhMuc)?.TenDanhMuc,
                UrlAnh = product.UrlAnh,
                MoTa = product.MoTa,
                SoLuongKho = product.SoLuongKho,
                GiaGoc = product.GiaGoc,
                GiaBan = product.GiaBan,
                SoLuongBan = product.SoLuongBan,
                TongDiemDG = danhGias.Sum(dg => dg.DiemDanhGia),
                SoLuotDG = danhGias.Count,
                DanhGias = danhGias
            };

            return View("DetailsProduct", viewModel);
        }

    }
}
