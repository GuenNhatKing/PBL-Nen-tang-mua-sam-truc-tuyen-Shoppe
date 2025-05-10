using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ManageAddress
    {
        [ValidateNever]
        public string IdNguoiDung { get; set; } = null!;
        [ValidateNever]
        public string? UrlAnhDaiDien { get; set; }
        public string HoVaTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<Thongtinlienhe> danhSachLienHe = new List<Thongtinlienhe>();
       
    }
}
