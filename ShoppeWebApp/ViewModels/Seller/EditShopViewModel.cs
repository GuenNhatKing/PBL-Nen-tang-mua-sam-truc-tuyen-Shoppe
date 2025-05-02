using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class EditShopViewModel
    {
        public string? IdCuaHang { get; set; } 

        [Required(ErrorMessage = "Tên cửa hàng không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên cửa hàng không được vượt quá 100 ký tự.")]
        public string? TenCuaHang { get; set; } 

        [Required(ErrorMessage = "ID chủ sở hữu không được để trống.")]
        public string? IdSeller { get; set; } 

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Sdt { get; set; } 

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string? DiaChi { get; set; } 

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? MoTa { get; set; } 

        public string? UrlAnhHienTai { get; set; } 

        [Display(Name = "Tải lên ảnh mới")]
        public IFormFile? UrlAnhMoi { get; set; } 
    }
}