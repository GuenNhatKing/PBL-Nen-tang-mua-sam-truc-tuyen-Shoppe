namespace ShoppeWebApp.ViewModels.Customer
{
    public class Profiles
    {
        public string IdNguoiDung { get; set; }
        public string? UrlAnhDaiDien { get; set; }
        public string HoVaTen { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public decimal SoDu { get; set; }
    }
}
