using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string PhoneNumber { get; set; }
    }
}
