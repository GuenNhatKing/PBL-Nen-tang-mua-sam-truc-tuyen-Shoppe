using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class CreateProductViewModel
    {
        public string? IdSanPham { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public string? IdDanhMuc { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string? TenSanPham { get; set; }

        [Required(ErrorMessage = "Url ảnh là bắt buộc.")]
        public IFormFile? UrlAnh { get; set; }

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Số lượng kho là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng kho phải là số không âm.")]
        public int? SoLuongKho { get; set; }

        [Required(ErrorMessage = "Giá gốc là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải là số không âm.")]
        public decimal? GiaGoc { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải là số không âm.")]
        public decimal? GiaBan { get; set; }

        public int TrangThai { get; set; } = 1; 
        public string? IdCuaHang { get; set; }
    }
}