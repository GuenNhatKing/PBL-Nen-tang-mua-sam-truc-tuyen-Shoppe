using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Customer;
using ShoppeWebApp.Services;

namespace ShoppeWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AllProductsController : Controller
    {
        private readonly ShoppeWebAppDbContext _context;
        public AllProductsController(ShoppeWebAppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, string? IdDanhMuc = null, string? searchString = null)
        {
            AllProductInfo products = new AllProductInfo();
            products.categories = await _context.Danhmucs.ToListAsync();
            products.danhMuc = IdDanhMuc;
            var query = _context.Sanphams.AsQueryable();

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
                products.productInfos.Add(new ItemInfo
                {
                    IdSanPham = i.IdSanPham,
                    TenSanPham = i.TenSanPham,
                    UrlAnh = i.UrlAnh,
                    GiaBan = i.GiaBan,
                    DiemDanhGia = i.SoLuongDanhGia > 0? i.TongDiemDanhGia / i.SoLuongDanhGia: 0,
                    SoLuongBan = AllProductInfo.ProcessQuantity(i.SoLuongBan),
                });
            }
            ViewBag.PagingInfo = PagingLoad.GetPaging(totalPage, page);
            ViewBag.Page = page;
            ViewBag.IdDanhMuc = IdDanhMuc;
            return View(products);
        }
    }
}

