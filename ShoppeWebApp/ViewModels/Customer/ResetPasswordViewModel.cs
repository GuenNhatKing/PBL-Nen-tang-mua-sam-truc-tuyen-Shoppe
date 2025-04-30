using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Cần nhập mật khẩu mới.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Cần xác nhận lại mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu đã nhập không khớp.")]
        public string ConfirmPassword { get; set; }

        public string Username { get; set; }
    }
}
