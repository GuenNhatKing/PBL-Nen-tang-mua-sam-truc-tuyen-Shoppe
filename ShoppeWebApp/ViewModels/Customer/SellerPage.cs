using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class SellerPage
    {
        public string IdCuaHang { get; set; } = null!;
        public string TenCuaHang { get; set; } = null!;
        public string UrlAnhCuaHang { get; set; } = null!;
        public int SoSanPhamDangBan { get; set; }
        public string SdtCuaHang { get; set; } = null!;
        public string DiaChiCuaHang { get; set; } = null!;
        public string? MoTaCuaHang { get; set; } = null!;
        public DateTime? ThoiGianThamGia { get; set; } = null!;
        public List<ItemInfo> productInfos = new List<ItemInfo>();
        public List<Danhmuc> categories = null!;
        public string? danhMuc = null;
    }
}
