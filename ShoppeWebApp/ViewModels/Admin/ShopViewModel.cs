namespace ShoppeWebApp.ViewModels.Admin
{
    public class ShopViewModel
    {
        public string? IdCuaHang { get; set; }
        public string? TenCuaHang { get; set; }
        public string? IdSeller { get; set; }
        public string? TenSeller { get; set; }
        public string? Sdt { get; set; }
        public string? UrlAnh { get; set; }
        public string? DiaChi { get; set; }
        public string? MoTa { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public DateTime? ThoiGianXoa { get; set; }
        public int SoSanPham { get; set; }
        public int SoDonHang { get; set; }
        public int TrangThai { get; set; }
    }
}