using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class AllProductInfo
    {
        public List<ProductInfo> productInfos;
        public List<DanhMuc> categories;
        public string danhMuc;
        public AllProductInfo(ShoppeWebAppContext context)
            :this(context, null!)
        {
            
        }
        public AllProductInfo(ShoppeWebAppContext context, string IdDanhMuc)
        {
            this.danhMuc = IdDanhMuc;
            productInfos = new List<ProductInfo>();
            categories = context.DanhMucs.ToList();
            var products = danhMuc == null ? context.SanPhams.ToList() : context.SanPhams.Where(i => i.IdDanhMuc == IdDanhMuc).ToList();
            foreach (var i in products)
            {
                productInfos.Add(new ProductInfo
                {
                    IdSanPham = i.IdSanPham,
                    TenSanPham = i.TenSanPham,
                    UrlAnh = i.UrlAnh,
                    GiaBan = i.GiaBan,
                    DiemDanhGia = i.TongDiemDg / i.SoLuotDg,
                    SoLuongBan = ProcessQuantity(i.SoLuongBan),
                });
            }
        }
        public static string ProcessQuantity(int quantity)
        {
            double curr = quantity;
            int expo = 0;
            string[] symbol = {"", "K", "M", "B", "T", "Q", "Qi"};
            while(curr >= 1000)
            {
                curr /= 1000;
                ++expo;
                if (expo >= symbol.Length) break;
            }
            string res = string.Format("{0:0.#}{1}", curr, symbol[expo]);
            return res;
        }
    }
    public class ProductInfo
    {
        public string IdSanPham { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public string UrlAnh { get; set; } = null!;
        public decimal GiaBan { get; set; }
        public decimal DiemDanhGia { get; set; }
        public string SoLuongBan { get; set; } = null!;
    }
}
