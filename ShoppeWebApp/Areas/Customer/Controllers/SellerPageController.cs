using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Services;
using ShoppeWebApp.ViewModels.Customer;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SellerPageController : Controller
    {
       private readonly ShoppeWebAppDbContext _context;
        public SellerPageController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, string? IdShop = null, string? IdDanhMuc = null, string? searchString = null)
        {
            if(IdShop == null)
            {
                return NotFound();
            }
            var shop = await _context.Cuahangs.FirstOrDefaultAsync(i => i.IdCuaHang == IdShop);
            if(shop == null)
            {
                return NotFound();
            }
            SellerPage sellerPage = new SellerPage
            {
                IdCuaHang = shop.IdCuaHang,
                TenCuaHang = shop.TenCuaHang,
                UrlAnhCuaHang = shop.UrlAnh,
                SoSanPhamDangBan = await _context.Sanphams.CountAsync(i => i.IdCuaHang == IdShop),
                DiaChiCuaHang = shop.DiaChi,
                SdtCuaHang = shop.Sdt,
                MoTaCuaHang = shop.MoTa,
                ThoiGianThamGia = shop.ThoiGianTao,
            };
            var danhMucCoSanPham = await _context.Sanphams
            .Where(i => i.IdCuaHang == IdShop)
            .GroupBy(i => new
            {
                IdDanhMuc = i.IdDanhMuc
            })
            .Select(i => i.Key.IdDanhMuc)
            .ToListAsync();
            sellerPage.categories = await _context.Danhmucs.Where(i => danhMucCoSanPham.Contains(i.IdDanhMuc)).ToListAsync();
            sellerPage.danhMuc = IdDanhMuc;
            var query = _context.Sanphams.Where(i => i.IdCuaHang == IdShop).AsQueryable();

            if(IdDanhMuc != null)
            {
                query = query.Where(i => i.IdDanhMuc == IdDanhMuc);
            }
            if(searchString != null)
            {
                searchString = searchString.ToUpper();
                query = query.Where(i => i.TenSanPham.ToUpper().Contains(searchString));
            }

            int totalProducts = await query.CountAsync();

            int totalPage = totalProducts % Constants.PAGINATION_SIZE == 0?
                totalProducts / Constants.PAGINATION_SIZE
                :totalProducts / Constants.PAGINATION_SIZE + 1;
            if (totalPage != 0 && (page < 1 || page > totalPage))
            {
                return NotFound();
            }

            var pros = await query.OrderBy(i => i.IdSanPham)
                .Skip((page - 1) * Constants.PAGINATION_SIZE)
                .Take(Constants.PAGINATION_SIZE).ToListAsync();

            foreach (var i in pros)
            {
                sellerPage.productInfos.Add(new ItemInfo
                {
                    IdSanPham = i.IdSanPham,
                    TenSanPham = i.TenSanPham,
                    UrlAnh = i.UrlAnh,
                    GiaBan = i.GiaBan,
                    DiemDanhGia = i.SoLuongDanhGia > 0? i.TongDiemDanhGia / i.SoLuongDanhGia: 0,
                    SoLuongBan = Quantity.ProcessQuantity(i.SoLuongBan),
                });
            }
            ViewBag.PagingInfo = PagingLoad.GetPaging(totalPage, page);
            ViewBag.Page = page;
            ViewBag.IdDanhMuc = IdDanhMuc;
            return View(sellerPage);
        }
    }
}
