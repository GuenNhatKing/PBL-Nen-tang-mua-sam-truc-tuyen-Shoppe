using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class EditShopViewModel
    {
        public string? IdCuaHang { get; set; } // ID của cửa hàng (không chỉnh sửa)

        [Required(ErrorMessage = "Tên cửa hàng không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên cửa hàng không được vượt quá 100 ký tự.")]
        public string? TenCuaHang { get; set; } // Tên cửa hàng

        [Required(ErrorMessage = "ID chủ sở hữu không được để trống.")]
        public string? IdSeller { get; set; } // ID chủ sở hữu (không chỉnh sửa)

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Sdt { get; set; } // Số điện thoại

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string? DiaChi { get; set; } // Địa chỉ

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? MoTa { get; set; } // Mô tả

        public string? UrlAnhHienTai { get; set; } // Đường dẫn ảnh hiện tại

        [Display(Name = "Tải lên ảnh mới")]
        public IFormFile? UrlAnhMoi { get; set; } // Ảnh mới (nếu có)
    }
}