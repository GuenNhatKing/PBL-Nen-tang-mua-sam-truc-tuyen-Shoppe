using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class EditShopViewModel
    {
        public string? IdCuaHang { get; set; }

        [Required(ErrorMessage = "Tên cửa hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên cửa hàng không được vượt quá 100 ký tự.")]
        public string? TenCuaHang { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Sdt { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string? DiaChi { get; set; }

        public string? MoTa { get; set; }

        public string? UrlAnhHienTai { get; set; }

        public IFormFile? UrlAnhMoi { get; set; }
    }
}