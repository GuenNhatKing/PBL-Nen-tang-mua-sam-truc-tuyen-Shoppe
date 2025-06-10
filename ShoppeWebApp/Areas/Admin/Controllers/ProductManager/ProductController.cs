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
                    TrangThai = p.TrangThai,
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
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.UrlAnh.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.UrlAnh.CopyTo(stream);
                }
                imagePath = $"images/products/{fileName}";
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

        [HttpGet]
        public IActionResult DeleteReview(string idDanhGia, string idSanPham, string IdCuaHang)
        {
            // Tìm đánh giá cần xóa
            var danhGia = _context.Danhgia.FirstOrDefault(d => d.IdDanhGia == idDanhGia);
        
            if (danhGia != null)
            {
                // Xóa đánh giá
                _context.Danhgia.Remove(danhGia);
                _context.SaveChanges();
            }
        
            // Chuyển hướng về trang AllReviews với các tham số cần thiết
            return RedirectToAction("AllReviews", new { idSanPham = idSanPham, IdCuaHang = IdCuaHang });
        } 
    
        [HttpGet]
        public IActionResult Edit(string idSanPham, string idCuaHang)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == idSanPham);
            if (sanPham == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            Console.WriteLine($"UrlAnh: {sanPham.UrlAnh}");
            // Lấy danh sách danh mục
            var danhSachDanhMuc = _context.Danhmucs
                .Select(dm => new ThongTinDanhMuc
                {
                    MaDanhMuc = dm.IdDanhMuc,
                    TenDanhMuc = dm.TenDanhMuc
                })
                .ToList();

            // Tạo ViewModel
            var viewModel = new ChinhSuaSanPhamViewModel
            {
                IdSanPham = sanPham.IdSanPham,
                IdCuaHang = idCuaHang,
                TenSanPham = sanPham.TenSanPham,
                GiaGoc = sanPham.GiaGoc,
                GiaBan = sanPham.GiaBan,
                SoLuongKho = sanPham.SoLuongKho,
                DuongDanAnh = sanPham.UrlAnh,
                MaDanhMucDuocChon = sanPham.IdDanhMuc,
                DanhSachDanhMuc = danhSachDanhMuc,
                MoTa = sanPham.MoTa,
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ChinhSuaSanPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, trả về View với thông tin lỗi
                model.DanhSachDanhMuc = _context.Danhmucs
                    .Select(dm => new ThongTinDanhMuc
                    {
                        MaDanhMuc = dm.IdDanhMuc,
                        TenDanhMuc = dm.TenDanhMuc
                    })
                    .ToList();
                return View(model);
            }

            // Lấy sản phẩm từ cơ sở dữ liệu
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == model.IdSanPham);
            if (sanPham == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            // Cập nhật thông tin sản phẩm
            sanPham.TenSanPham = model.TenSanPham;
            sanPham.GiaGoc = model.GiaGoc;
            sanPham.GiaBan = model.GiaBan;
            sanPham.SoLuongKho = model.SoLuongKho;
            sanPham.IdDanhMuc = model.MaDanhMucDuocChon;
            sanPham.MoTa = model.MoTa;

            // Xử lý ảnh mới nếu có
            if (model.AnhMoi != null && model.AnhMoi.Length > 0)
            {
                // Đường dẫn lưu tệp
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
            
                // Tạo tên tệp duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AnhMoi.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
                // Lưu tệp vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.AnhMoi.CopyTo(fileStream);
                }
            
                // Xóa ảnh cũ nếu cần
                if (!string.IsNullOrEmpty(sanPham.UrlAnh) && sanPham.UrlAnh != "images/products/default.png")
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sanPham.UrlAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            
                // Cập nhật đường dẫn ảnh mới vào cơ sở dữ liệu
                sanPham.UrlAnh = $"images/products/{uniqueFileName}";
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công.";
            return RedirectToAction("Edit", new { idSanPham = model.IdSanPham, idCuaHang = model.IdCuaHang });
        }

        [HttpGet]
        public IActionResult Delete(string idSanPham, string idCuaHang)
        {
            // Tìm sản phẩm theo ID
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == idSanPham && sp.IdCuaHang == idCuaHang);
            if (sanPham == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
            }
        
            // Lấy thông tin danh mục
            var danhMuc = _context.Danhmucs.FirstOrDefault(dm => dm.IdDanhMuc == sanPham.IdDanhMuc);
        
            // Kiểm tra xem sản phẩm có liên quan đến đơn hàng hay không
            var hasOrders = _context.Chitietdonhangs.Any(ct => ct.IdSanPham == idSanPham);
            if (hasOrders)
            {
                ViewBag.WarningMessage = "Sản phẩm này đang có trong đơn hàng. Việc xóa có thể ảnh hưởng đến dữ liệu đơn hàng.";
            }
        
            // Tính lại tổng điểm đánh giá và số lượt đánh giá
            var danhGias = _context.Danhgia.Where(dg => dg.IdSanPham == idSanPham).ToList();
            sanPham.TongDiemDanhGia = danhGias.Sum(dg => dg.DiemDanhGia);
            sanPham.SoLuongDanhGia = danhGias.Count;
        
            // Tạo ViewModel để truyền dữ liệu sang View
            var viewModel = new DetailsProductViewModel
            {
                IdSanPham = sanPham.IdSanPham,
                IdCuaHang = sanPham.IdCuaHang,
                TenSanPham = sanPham.TenSanPham,
                TenDanhMuc = danhMuc?.TenDanhMuc,
                UrlAnh = sanPham.UrlAnh,
                MoTa = sanPham.MoTa,
                SoLuongKho = sanPham.SoLuongKho,
                GiaGoc = sanPham.GiaGoc,
                GiaBan = sanPham.GiaBan,
                TrangThai = sanPham.TrangThai,
                TongDiemDG = sanPham.TongDiemDanhGia,
                SoLuotDG = sanPham.SoLuongDanhGia,
                SoLuongBan = sanPham.SoLuongBan,
                ThoiGianTao = sanPham.ThoiGianTao
            };
        
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string idSanPham, string idCuaHang)
        {
            // Tìm sản phẩm cần cập nhật trạng thái
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == idSanPham && sp.IdCuaHang == idCuaHang);
            if (sanPham == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
            }
        
            try
            {
                // Cập nhật trạng thái sản phẩm thành 0 (không hoạt động)
                sanPham.TrangThai = 0;
        
                // Ghi nhận thời gian xóa
                sanPham.ThoiGianXoa = DateTime.Now;
        
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi cập nhật trạng thái sản phẩm: {ex.Message}";
            }
            return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
        }
        
        private bool CanRestoreProduct(DateTime? thoiGianXoa)
        {
            if (!thoiGianXoa.HasValue)
            {
                return true; // Nếu chưa bị xóa, có thể khôi phục
            }
        
            // Kiểm tra nếu thời gian xóa vượt quá 30 ngày
            return (DateTime.Now - thoiGianXoa.Value).TotalDays <= 30;
        }
        [HttpGet]
        public IActionResult DetailsProductDelete(string idSanPham, string idCuaHang)
        {
            // Tìm sản phẩm theo ID
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == idSanPham && sp.IdCuaHang == idCuaHang);
            if (sanPham == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
            }
        
            // Lấy thông tin danh mục
            var danhMuc = _context.Danhmucs.FirstOrDefault(dm => dm.IdDanhMuc == sanPham.IdDanhMuc);

            // Tính lại tổng điểm đánh giá và số lượt đánh giá
            var danhGias = _context.Danhgia.Where(dg => dg.IdSanPham == idSanPham).ToList();
            sanPham.TongDiemDanhGia = danhGias.Sum(dg => dg.DiemDanhGia);
            sanPham.SoLuongDanhGia = danhGias.Count;
            // Kiểm tra xem sản phẩm có thể khôi phục hay không
            var canRestore = CanRestoreProduct(sanPham.ThoiGianXoa);

            // Tạo ViewModel để truyền dữ liệu sang View
            var viewModel = new DetailsProductViewModel
            {
                IdSanPham = sanPham.IdSanPham,
                IdCuaHang = sanPham.IdCuaHang,
                TenSanPham = sanPham.TenSanPham,
                TenDanhMuc = danhMuc?.TenDanhMuc,
                UrlAnh = sanPham.UrlAnh,
                MoTa = sanPham.MoTa,
                SoLuongKho = sanPham.SoLuongKho,
                GiaGoc = sanPham.GiaGoc,
                GiaBan = sanPham.GiaBan,
                TrangThai = canRestore ? 1 : 0, // Nếu không thể khôi phục, trạng thái là 0
                TongDiemDG = sanPham.TongDiemDanhGia,
                SoLuotDG = sanPham.SoLuongDanhGia,
                SoLuongBan = sanPham.SoLuongBan,
                ThoiGianTao = sanPham.ThoiGianTao,
                ThoiGianXoa = sanPham.ThoiGianXoa
            };
        
            return View(viewModel);
        }    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(string idSanPham, string idCuaHang)
        {
            // Tìm sản phẩm cần khôi phục
            var sanPham = _context.Sanphams.FirstOrDefault(sp => sp.IdSanPham == idSanPham && sp.IdCuaHang == idCuaHang);
            if (sanPham == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
            }
        
            try
            {
                // Cập nhật trạng thái sản phẩm thành 1 (hoạt động)
                sanPham.TrangThai = 1;
        
                // Xóa thời gian xóa (nếu có)
                sanPham.ThoiGianXoa = null;
        
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
        
                TempData["SuccessMessage"] = "Sản phẩm đã được khôi phục thành công.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi khôi phục sản phẩm: {ex.Message}";
            }
        
            return RedirectToAction("Index", new { IdCuaHang = idCuaHang });
        }
    }
}