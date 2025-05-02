using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.ViewModels.Seller;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using System.Linq;

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
        public IActionResult Create()
        {
            var idCuaHang = User.Claims.FirstOrDefault(c => c.Type == "IdCuaHang")?.Value;

            if (string.IsNullOrEmpty(idCuaHang))
            {
                TempData["ErrorMessage"] = "Không thể xác định cửa hàng của bạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var model = new CreateProductViewModel
            {
                IdCuaHang = idCuaHang
            };

            // Lấy danh sách danh mục
            ViewBag.Categories = _context.Danhmucs
                .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Danhmucs
                    .Select(c => new { c.IdDanhMuc, c.TenDanhMuc })
                    .ToList();
                return View(model);
            }

            // Upload ảnh
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

            // Tạo sản phẩm mới
            var product = new Sanpham
            {
                IdSanPham = Guid.NewGuid().ToString(),
                IdCuaHang = model.IdCuaHang,
                IdDanhMuc = model.IdDanhMuc,
                TenSanPham = model.TenSanPham,
                UrlAnh = imagePath,
                MoTa = model.MoTa,
                SoLuongKho = model.SoLuongKho ?? 0,
                GiaGoc = model.GiaGoc ?? 0,
                GiaBan = model.GiaBan ?? 0,
                TrangThai = 1,
                ThoiGianTao = DateTime.Now
            };

            _context.Sanphams.Add(product);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index", new { IdCuaHang = model.IdCuaHang });
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