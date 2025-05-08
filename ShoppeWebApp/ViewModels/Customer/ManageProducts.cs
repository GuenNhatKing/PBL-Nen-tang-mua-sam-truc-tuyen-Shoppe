using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ManageProducts
    {
        [ValidateNever]
        public string IdNguoiDung { get; set; } = null!;
        [ValidateNever]
        public string? UrlAnhDaiDien { get; set; }
        public string HoVaTen { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        [ValidateNever]
        public decimal SoDu { get; set; }
    }
}
