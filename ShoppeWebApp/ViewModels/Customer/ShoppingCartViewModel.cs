using ShoppeWebApp.Models;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartShopProducts> danhSachCuaHang = new List<ShoppingCartShopProducts>();
        public List<Thongtinlienhe> ThongTinLienHe = new List<Thongtinlienhe>();
    }
    public class ShoppingCartShopProducts
    {
        public string IdCuaHang { get; set; } = null!;
        public string? TenCuaHang { get; set; } = null!;
        public List<ShoppingCartProductInfo> danhSachSanPham = new List<ShoppingCartProductInfo>();
    }
    public class ShoppingCartProductInfo
    {
        public string IdSanPham { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public string UrlAnh { get; set; } = null!;
        public decimal GiaGoc { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongKho { get; set; }
    }
}
