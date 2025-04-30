namespace ShoppeWebApp.ViewModels.Customer
{
    public class ItemInfo
    {
        public string IdSanPham { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public string UrlAnh { get; set; } = null!;
        public decimal GiaBan { get; set; }
        public decimal DiemDanhGia { get; set; }
        public string SoLuongBan { get; set; } = null!;
    }
}
