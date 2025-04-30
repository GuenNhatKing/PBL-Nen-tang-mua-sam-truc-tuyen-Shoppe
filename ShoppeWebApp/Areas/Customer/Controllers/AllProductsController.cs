using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;
using ShoppeWebApp.ViewModels.Customer;

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
            var pros = IdDanhMuc == null?
                await _context.Sanphams
                .OrderBy(i => i.IdSanPham)
                .Skip((page - 1) * Constants.PAGINATION_SIZE)
                .Take(Constants.PAGINATION_SIZE).ToListAsync() :
                await _context.Sanphams.Where(i => i.IdDanhMuc == IdDanhMuc)
                .OrderBy(i => i.IdSanPham)
                .Skip((page - 1) * Constants.PAGINATION_SIZE)
                .Take(Constants.PAGINATION_SIZE).ToListAsync();
            int count = 0;
            foreach (var i in pros)
            {
                ++count;
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
            ViewBag.TotalPage = count;
            ViewBag.Page = page;
            return View(products);
        }
    }
}

