using System.ComponentModel.DataAnnotations;
namespace ShoppeWebApp.ViewModels.Seller
{
    public class ShopViewModel
    {
        public string? IdCuaHang { get; set; }

        [Required(ErrorMessage = "Tên cửa hàng không được để trống.")]
        public string? TenCuaHang { get; set; }

        public string? IdSeller { get; set; }
        public string? TenSeller { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Sdt { get; set; }

        public string? UrlAnh { get; set; }
        public string? DiaChi { get; set; }
        public string? MoTa { get; set; }
        public DateTime? ThoiGianTao { get; set; }

        public int SoSanPham { get; set; }
        public int SoDonHang { get; set; }
    }
}