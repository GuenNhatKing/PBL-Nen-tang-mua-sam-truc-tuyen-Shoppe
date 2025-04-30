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
        public async Task<IActionResult> Index(int page = 1, string? IdDanhMuc = null)
        {
            AllProductInfo products = new AllProductInfo();
            products.categories = await _context.Danhmucs.ToListAsync();
            products.danhMuc = IdDanhMuc;

            int totalProducts = IdDanhMuc == null ?
                await _context.Sanphams.CountAsync()
                : await _context.Sanphams.CountAsync(i => i.IdDanhMuc == IdDanhMuc);

            int totalPage = totalProducts % Constants.PAGINATION_SIZE == 0?
                totalProducts / Constants.PAGINATION_SIZE
                :totalProducts / Constants.PAGINATION_SIZE + 1;
            Console.WriteLine($"Total products: {totalProducts}");
            Console.WriteLine($"Total pages: {totalPage}");
            if (totalPage != 0 && (page < 1 || page > totalPage))
            {
                return NotFound();
            }

            var pros = IdDanhMuc == null?
                await _context.Sanphams
                .OrderBy(i => i.IdSanPham)
                .Skip((page - 1) * Constants.PAGINATION_SIZE)
                .Take(Constants.PAGINATION_SIZE).ToListAsync() :
                await _context.Sanphams.Where(i => i.IdDanhMuc == IdDanhMuc)
                .OrderBy(i => i.IdSanPham)
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
                    DiemDanhGia = i.TongDiemDanhGia / i.SoLuongDanhGia,
                    SoLuongBan = AllProductInfo.ProcessQuantity(i.SoLuongBan),
                });
            }
            ViewBag.PagingInfo = PagingLoad.GetPaging(totalPage, page);
            ViewBag.Page = page;
            return View(products);
        }
    }
}

