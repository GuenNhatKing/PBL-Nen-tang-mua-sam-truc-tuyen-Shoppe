using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên chủ tài khoản là bắt buộc.")]
        [Display(Name = "Tên chủ tài khoản")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string Sdt { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string XacNhanMatKhau { get; set; }
    }
}