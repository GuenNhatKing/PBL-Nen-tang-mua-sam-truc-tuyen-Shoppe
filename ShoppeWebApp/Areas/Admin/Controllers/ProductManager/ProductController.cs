using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.ViewModels.Admin;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models; 
using System.Linq;

namespace ShoppeWebApp.Areas.Admin.Controllers.ProductManager
{
    [Authorize("Admin")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public ProductController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string IdCuaHang, string IdDanhMuc, string searchQuery, int page = 1, int pageSize = 12)
        {
            // Lấy thông tin cửa hàng theo IdCuaHang
            var cuaHang = _context.Cuahangs.FirstOrDefault(ch => ch.IdCuaHang == IdCuaHang);

        
            // Lấy danh sách danh mục chỉ thuộc về các sản phẩm trong cửa hàng
            var categories = _context.Sanphams
                .Where(p => p.IdCuaHang == IdCuaHang && !string.IsNullOrEmpty(p.IdDanhMuc)) // Lọc theo IdCuaHang
                .Select(p => p.IdDanhMuc)
                .Distinct()
                .Join(_context.Danhmucs, id => id, c => c.IdDanhMuc, (id, c) => new CategoryInfo
                {
                    IdDanhMuc = c.IdDanhMuc,
                    TenDanhMuc = c.TenDanhMuc
                })
                .ToList();
            
            // Kiểm tra nếu danh mục rỗng
            if (!categories.Any())
            {
                var emptyViewModel = new ProductViewModel
                {
                    productInfos = new List<ProductInfo>(), // Danh sách sản phẩm rỗng
                    categories = new List<CategoryInfo>(), // Danh sách danh mục rỗng
                    danhMuc = null,
                    IdCuaHang = IdCuaHang,
                    TenCuaHang = cuaHang.TenCuaHang,
                    DiaChi = cuaHang.DiaChi,
                    SoDienThoai = cuaHang.Sdt,
                    UrlAnhCuaHang = cuaHang.UrlAnh
                };
            
                return View(emptyViewModel);
            }
        
            // Lấy danh sách sản phẩm theo danh mục, tìm kiếm và IdCuaHang
            var query = _context.Sanphams.Where(p => p.IdCuaHang == IdCuaHang).AsQueryable();
            if (!string.IsNullOrEmpty(IdDanhMuc))
            {
                query = query.Where(p => p.IdDanhMuc == IdDanhMuc);
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.IdSanPham.Contains(searchQuery) || 
                                         p.TenSanPham.Contains(searchQuery));
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
                    TyLeGiamGia = p.GiaGoc > 0 ? (int)((1 - (p.GiaBan / p.GiaGoc)) * 100) : 0, // Tính tỷ lệ giảm giá
                    SoLuongBan = p.SoLuongBan,
                    UrlAnh = p.UrlAnh
                })
                .ToList();
        
            // Tạo ViewModel
            var viewModel = new ProductViewModel
            {
                productInfos = products,
                categories = categories,
                danhMuc = IdDanhMuc,
                IdCuaHang = IdCuaHang,
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
        public IActionResult DetailsProduct(string IdSanPham, string IdCuaHang)
        {
            // Lấy thông tin sản phẩm
            var product = _context.Sanphams.FirstOrDefault(p => p.IdSanPham == IdSanPham && p.IdCuaHang == IdCuaHang);
            if (product == null)
            {
                return NotFound();
            }
        
            // Lấy thông tin danh mục
            var danhMuc = _context.Danhmucs.FirstOrDefault(dm => dm.IdDanhMuc == product.IdDanhMuc);

            var viewModel = new DetailsProductViewModel
            {
                IdSanPham = product.IdSanPham,
                IdCuaHang = product.IdCuaHang,
                TenSanPham = product.TenSanPham,
                TenDanhMuc = danhMuc?.TenDanhMuc,
                UrlAnh = product.UrlAnh,
                MoTa = product.MoTa,
                SoLuongKho = product.SoLuongKho,
                GiaGoc = product.GiaGoc,
                GiaBan = product.GiaBan,
                TrangThai = product.TrangThai,
                SoLuongBan = product.SoLuongBan,
                ThoiGianTao = product.ThoiGianTao
            };
        
            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult AllReviews(string idSanPham, string IdCuaHang, string? searchByIdDanhGia, string? searchByIdNguoiDung, string? filterByStars)
        {
            var danhGiasQuery = _context.Danhgia.AsQueryable();
        
            // Lọc theo ID sản phẩm
            if (!string.IsNullOrEmpty(idSanPham))
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.IdSanPham == idSanPham);
            }
        
            // Lọc theo ID đánh giá
            if (!string.IsNullOrEmpty(searchByIdDanhGia))
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.IdDanhGia.Contains(searchByIdDanhGia));
            }
        
            // Lọc theo ID người dùng
            if (!string.IsNullOrEmpty(searchByIdNguoiDung))
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.IdNguoiDung.Contains(searchByIdNguoiDung));
            }
        
            // Lọc theo số sao
            if (!string.IsNullOrEmpty(filterByStars) && int.TryParse(filterByStars, out int stars))
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.DiemDanhGia == stars);
            }
        
            // Thực hiện join sau khi lọc
            var danhGias = danhGiasQuery
                .Join(
                    _context.Nguoidungs,
                    danhGia => danhGia.IdNguoiDung,
                    nguoiDung => nguoiDung.IdNguoiDung,
                    (danhGia, nguoiDung) => new AllReviewsViewModel.ReviewViewModel
                    {
                        IdDanhGia = danhGia.IdDanhGia,
                        IdNguoiDung = danhGia.IdNguoiDung,
                        TenNguoiDung = nguoiDung.HoVaTen,
                        DiemDanhGia = danhGia.DiemDanhGia,
                        NoiDung = danhGia.NoiDung,
                        ThoiGianDG = danhGia.ThoiGianDg
                    }
                )
                .ToList();
        
            // Tạo ViewModel
            var viewModel = new AllReviewsViewModel
            {
                IdSanPham = idSanPham,
                IdCuaHang = IdCuaHang,
                FilterByStars = filterByStars, 
                DanhSachDanhGia = danhGias
            };
        
            return View(viewModel);
        }
    }
}