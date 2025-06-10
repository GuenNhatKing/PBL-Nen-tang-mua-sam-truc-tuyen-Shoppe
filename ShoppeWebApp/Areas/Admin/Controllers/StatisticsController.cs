using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeWebApp.Data;
using ShoppeWebApp.ViewModels.Admin;
using System.Linq;
using ShoppeWebApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;

namespace ShoppeWebApp.Areas.Admin.Controllers
{
    [Authorize("Admin")]
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;

        public StatisticsController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult TotalRevenue(DateTime? StartDate, DateTime? EndDate)
        {

            // ✅ Kiểm tra ngày bắt đầu - kết thúc hợp lệ
            if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            }

            // ✅ Kiểm tra ngày bắt đầu không vượt quá ngày hiện tại
            if (StartDate.HasValue && StartDate.Value.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu không được lớn hơn ngày hiện tại.");
            }

            // Lấy danh sách đơn hàng trong khoảng thời gian
            var orders = _context.Donhangs.AsQueryable();
        
            if (StartDate.HasValue)
            {
                orders = orders.Where(o => o.ThoiGianTao >= StartDate.Value);
            }
        
            if (EndDate.HasValue)
            {
                orders = orders.Where(o => o.ThoiGianTao <= EndDate.Value);
            }
        
            // Tính tổng doanh thu
            var totalRevenue = orders.Where(o => o.TrangThai == Constants.DA_GIAO).Sum(o => o.TongTien);
        
            // Thống kê trạng thái đơn hàng
            var totalOrders = orders.Count();
            var pendingOrders = orders.Count(o => o.TrangThai == Constants.CHO_XAC_NHAN);
            var confirmedOrders = orders.Count(o => o.TrangThai == Constants.DA_XAC_NHAN);
            var deliveredOrders = orders.Count(o => o.TrangThai == Constants.DA_GIAO);
            var cancelledOrders = orders.Count(o => o.TrangThai == Constants.HUY_DON_HANG);
        
            // Thống kê trạng thái sản phẩm
            var activeProducts = _context.Sanphams.Count(p => p.TrangThai == Constants.CON_HANG);
            var temporarilyLockedProducts = _context.Sanphams.Count(p => p.TrangThai == Constants.TAM_KHOA);
        
            // Dữ liệu biểu đồ doanh thu theo ngày
            var revenueData = orders
                .GroupBy(o => o.ThoiGianTao.HasValue ? o.ThoiGianTao.Value.Date : DateTime.MinValue)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TongTien) })
                .OrderBy(g => g.Date)
                .ToList();
        
            var revenueDates = revenueData.Select(d => d.Date).ToList();
            var revenueValues = revenueData.Select(d => d.Revenue).ToList();
        
            // Dữ liệu biểu đồ tần suất trạng thái đơn hàng
            var orderFrequencies = orders
                .GroupBy(o => o.ThoiGianTao.HasValue ? o.ThoiGianTao.Value.Date : DateTime.MinValue)
                .Select(g => g.Count())
                .ToList();
        
            var cancelledFrequencies = orders
                .Where(o => o.TrangThai == Constants.HUY_DON_HANG)
                .GroupBy(o => o.ThoiGianTao.HasValue ? o.ThoiGianTao.Value.Date : DateTime.MinValue)
                .Select(g => g.Count())
                .ToList();
        
            var deliveredFrequencies = orders
                .Where(o => o.TrangThai == Constants.DA_GIAO)
                .GroupBy(o => o.ThoiGianTao.HasValue ? o.ThoiGianTao.Value.Date : DateTime.MinValue)
                .Select(g => g.Count())
                .ToList();
        
            // Truyền dữ liệu sang ViewModel
            var viewModel = new TotalRevenueViewModel
            {
                StartDate = StartDate,
                EndDate = EndDate,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                ConfirmedOrders = confirmedOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                ActiveProducts = activeProducts,
                TemporarilyLockedProducts = temporarilyLockedProducts,
                RevenueDates = revenueDates,
                RevenueValues = revenueValues,
                OrderFrequencies = orderFrequencies,
                CancelledFrequencies = cancelledFrequencies,
                DeliveredFrequencies = deliveredFrequencies
            };
        
            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult TopSellingProducts(DateTime? startDate, DateTime? endDate, string? selectedCategory)
        {
            // ✅ Kiểm tra ngày bắt đầu - kết thúc hợp lệ
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            }

            // ✅ Kiểm tra ngày bắt đầu không vượt quá ngày hiện tại
            if (startDate.HasValue && startDate.Value.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu không được lớn hơn ngày hiện tại.");
            }

            // Lấy danh sách các danh mục sản phẩm
            var availableCategories = _context.Danhmucs
                .Select(c => c.TenDanhMuc)
                .ToList();
        
            // Lấy danh sách các sản phẩm và tính doanh thu trong khoảng thời gian
            var productsQuery = _context.Sanphams
                .Select(p => new
                {
                    ProductId = p.IdSanPham, // Lấy đúng ProductId từ bảng Sanphams
                    ProductName = p.TenSanPham,
                    ImageUrl = p.UrlAnh,
                    CategoryName = _context.Danhmucs
                        .Where(c => c.IdDanhMuc == p.IdDanhMuc)
                        .Select(c => c.TenDanhMuc)
                        .FirstOrDefault(),
                    Revenue = _context.Donhangs
                        .Where(o => o.TrangThai == Constants.DA_GIAO // Chỉ tính doanh thu từ đơn hàng đã giao
                                    && o.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                    && o.ThoiGianTao <= (endDate ?? DateTime.MaxValue)) // Lọc theo ngày kết thúc
                        .Where(o => o.Chitietdonhangs.Any(ct => ct.IdSanPham == p.IdSanPham)) // Liên kết sản phẩm với đơn hàng
                        .Sum(o => o.Chitietdonhangs
                            .Where(ct => ct.IdSanPham == p.IdSanPham)
                            .Sum(ct => ct.SoLuong * ct.DonGia)), // Tính tổng doanh thu từ chi tiết đơn hàng
                    QuantitySold = _context.Chitietdonhangs
                        .Where(ct => ct.IdSanPham == p.IdSanPham // Liên kết sản phẩm với chi tiết đơn hàng
                                     && _context.Donhangs.Any(o => o.IdDonHang == ct.IdDonHang // Đơn hàng liên quan
                                                                    && o.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                                                    && o.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                                                    && o.ThoiGianTao <= (endDate ?? DateTime.MaxValue))) // Lọc theo ngày kết thúc
                        .Sum(ct => ct.SoLuong), // Tính tổng số lượng đã bán
                    OrderCount = _context.Donhangs
                        .Where(o => o.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                    && o.ThoiGianTao >= (startDate ?? DateTime.MinValue)
                                    && o.ThoiGianTao <= (endDate ?? DateTime.MaxValue)
                                    && o.Chitietdonhangs.Any(ct => ct.IdSanPham == p.IdSanPham)) // Liên kết sản phẩm với đơn hàng
                        .Count() // Tính số lượng đơn hàng
                })
                .Where(p => p.Revenue > 0); // Chỉ lấy sản phẩm có doanh thu lớn hơn 0
        
            // Lọc theo danh mục nếu có
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                productsQuery = productsQuery.Where(p => p.CategoryName == selectedCategory);
            }
        
            // Lấy danh sách sản phẩm bán chạy
            var topSellingProducts = productsQuery
                .OrderByDescending(p => p.Revenue) // Sắp xếp theo doanh thu giảm dần
                .Take(10) // Lấy 10 sản phẩm có doanh thu cao nhất
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    QuantitySold = p.QuantitySold,
                    TotalRevenue = p.Revenue,
                    OrderCount = p.OrderCount // Số lượng đơn hàng
                })
                .ToList();
        
            // Tính doanh thu theo danh mục
            var categoryRevenues = productsQuery
                .GroupBy(p => p.CategoryName) // Nhóm theo tên danh mục
                .Select(g => new CategoryRevenue
                {
                    CategoryName = g.Key ?? "Unknown", // Tên danh mục
                    Revenue = g.Sum(p => p.Revenue) // Tổng doanh thu của danh mục
                })
                .ToList();
        
            // Tìm danh mục có doanh thu cao nhất
            var topCategory = categoryRevenues.OrderByDescending(c => c.Revenue).FirstOrDefault();
        
            // Truyền dữ liệu vào ViewModel
            var viewModel = new TopSellingProductsViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SelectedCategory = selectedCategory,
                AvailableCategories = availableCategories,
                TopSellingProducts = topSellingProducts,
                CategoryRevenues = categoryRevenues,
                TopCategoryName = topCategory?.CategoryName ?? "Không có",
                TopCategoryRevenue = topCategory?.Revenue ?? 0
            };
        
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RevenueByStore(DateTime? startDate, DateTime? endDate)
        {
            // ✅ Kiểm tra ngày bắt đầu - kết thúc hợp lệ
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            }
        
            // ✅ Kiểm tra ngày bắt đầu không vượt quá ngày hiện tại
            if (startDate.HasValue && startDate.Value.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu không được lớn hơn ngày hiện tại.");
            }
        
            // Lấy danh sách các cửa hàng và tính toán doanh thu, số lượng đơn hàng, số sản phẩm đã bán
            var storeRevenues = _context.Cuahangs
                .Select(store => new
                {
                    StoreId = store.IdCuaHang,
                    StoreName = store.TenCuaHang,
                    Revenue = _context.Donhangs
                        .Where(order => order.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == store.IdCuaHang) // Liên kết đơn hàng với cửa hàng
                                        && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                        && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                        && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue)) // Lọc theo ngày kết thúc
                        .Sum(order => order.TongTien), // Tính tổng doanh thu
                    OrderCount = _context.Donhangs
                        .Count(order => order.Chitietdonhangs.Any(ct => ct.IdSanPhamNavigation.IdCuaHang == store.IdCuaHang) // Liên kết đơn hàng với cửa hàng
                                        && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                        && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                        && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue)), // Lọc theo ngày kết thúc
                    ProductCount = _context.Chitietdonhangs
                        .Where(detail => _context.Donhangs
                            .Any(order => order.IdDonHang == detail.IdDonHang // Liên kết chi tiết đơn hàng với đơn hàng
                                          && detail.IdSanPhamNavigation.IdCuaHang == store.IdCuaHang // Liên kết sản phẩm với cửa hàng
                                          && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                          && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                          && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue))) // Lọc theo ngày kết thúc
                        .Select(detail => detail.IdSanPham) // Lấy danh sách ID sản phẩm
                        .Distinct() // Loại bỏ các sản phẩm trùng lặp
                        .Count() // Đếm số lượng sản phẩm khác nhau đã bán
                })
                .Where(store => store.Revenue > 0) // Chỉ lấy cửa hàng có doanh thu
                .OrderByDescending(store => store.Revenue) // Sắp xếp theo doanh thu giảm dần
                .ToList();
        
            // Truyền dữ liệu vào ViewModel
            var viewModel = new RevenueByStoreViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                StoreRevenues = storeRevenues.Select(s => new StoreRevenue
                {
                    StoreId = s.StoreId,
                    StoreName = s.StoreName,
                    Revenue = s.Revenue,
                    OrderCount = s.OrderCount,
                    ProductCount = s.ProductCount // Số sản phẩm đã bán
                }).ToList()
            };
        
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RevenueByCustomer(DateTime? startDate, DateTime? endDate)
        {
            // ✅ Kiểm tra ngày bắt đầu - kết thúc hợp lệ
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
            }
        
            // ✅ Kiểm tra ngày bắt đầu không vượt quá ngày hiện tại
            if (startDate.HasValue && startDate.Value.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, "Ngày bắt đầu không được lớn hơn ngày hiện tại.");
            }
        
            // Lấy danh sách người dùng có trạng thái 1 (đại diện cho khách hàng) và tính toán doanh thu, số lượng đơn hàng, số sản phẩm đã mua
            var customerRevenues = _context.Nguoidungs
                .Where(user => user.TrangThai == 1) // Chỉ lấy người dùng có trạng thái 1
                .Select(customer => new
                {
                    CustomerId = customer.IdNguoiDung,
                    CustomerName = customer.HoVaTen,
                    Revenue = _context.Donhangs
                        .Where(order => order.IdLienHeNavigation.IdNguoiDung == customer.IdNguoiDung // Liên kết đơn hàng với khách hàng
                                        && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                        && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                        && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue)) // Lọc theo ngày kết thúc
                        .Sum(order => order.TongTien), // Tính tổng doanh thu
                    OrderCount = _context.Donhangs
                        .Count(order => order.IdLienHeNavigation.IdNguoiDung == customer.IdNguoiDung // Liên kết đơn hàng với khách hàng
                                        && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                        && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                        && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue)), // Lọc theo ngày kết thúc
                    ProductCount = _context.Chitietdonhangs
                        .Where(detail => _context.Donhangs
                            .Any(order => order.IdDonHang == detail.IdDonHang // Liên kết chi tiết đơn hàng với đơn hàng
                                          && order.IdLienHeNavigation.IdNguoiDung == customer.IdNguoiDung // Liên kết đơn hàng với khách hàng
                                          && order.TrangThai == Constants.DA_GIAO // Chỉ tính đơn hàng đã giao
                                          && order.ThoiGianTao >= (startDate ?? DateTime.MinValue) // Lọc theo ngày bắt đầu
                                          && order.ThoiGianTao <= (endDate ?? DateTime.MaxValue))) // Lọc theo ngày kết thúc
                        .Sum(detail => detail.SoLuong) // Tính tổng số lượng sản phẩm đã mua
                })
                .Where(customer => customer.Revenue > 0) // Chỉ lấy khách hàng có doanh thu
                .OrderByDescending(customer => customer.Revenue) // Sắp xếp theo doanh thu giảm dần
                .ToList();
        
            // Truyền dữ liệu vào ViewModel
            var viewModel = new RevenueByCustomerViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                CustomerRevenues = customerRevenues.Select(c => new CustomerRevenue
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Revenue = c.Revenue,
                    OrderCount = c.OrderCount,
                    ProductCount = c.ProductCount
                }).ToList(),
                TotalCustomers = customerRevenues.Count // Tính tổng số khách hàng
            };
        
            return View(viewModel);
        }
    }
}