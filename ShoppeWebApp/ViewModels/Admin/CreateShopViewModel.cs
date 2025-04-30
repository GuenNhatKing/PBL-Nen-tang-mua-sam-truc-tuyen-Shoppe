using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class CreateShopViewModel
    {
        public string? IdCuaHang { get; set; } // ID của cửa hàng (không chỉnh sửa)
        [Required(ErrorMessage = "Tên cửa hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên cửa hàng không được vượt quá 100 ký tự.")]
        public string? TenCuaHang { get; set; } // Tên cửa hàng

        public string? IdSeller { get; set; } //  ID của chủ Shop (không chỉnh sửa)

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "SĐT phải có từ 10 ký tự.")]
        public string? Sdt { get; set; } // Số điện thoại liên hệ

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string? DiaChi { get; set; } // Địa chỉ cửa hàng

        public string? MoTa { get; set; }
        public IFormFile? UrlAnh { get; set; } 
    }
}