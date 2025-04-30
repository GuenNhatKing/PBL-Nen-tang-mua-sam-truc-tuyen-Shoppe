using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.ViewModels.Admin;
using ShoppeWebApp.Data;
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
        public IActionResult Index(string IdDanhMuc, string searchQuery, int page = 1, int pageSize = 12)
        {
            // Lấy thông tin cửa hàng 
            var cuaHang = _context.Cuahangs.FirstOrDefault(); // Lấy thông tin cửa hàng đầu tiên
            if (cuaHang == null)
            {
                return NotFound("Không tìm thấy thông tin cửa hàng.");
            }

            // Lấy danh sách danh mục chỉ thuộc về các sản phẩm trong Shop
            var categories = _context.Sanphams
                .Where(p => !string.IsNullOrEmpty(p.IdDanhMuc)) // Chỉ lấy sản phẩm có danh mục
                .Select(p => p.IdDanhMuc)
                .Distinct()
                .Join(_context.Danhmucs, id => id, c => c.IdDanhMuc, (id, c) => new CategoryInfo
                {
                    IdDanhMuc = c.IdDanhMuc,
                    TenDanhMuc = c.TenDanhMuc
                })
                .ToList();
        
            // Lấy danh sách sản phẩm theo danh mục và tìm kiếm
            var query = _context.Sanphams.AsQueryable();
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
    }
}