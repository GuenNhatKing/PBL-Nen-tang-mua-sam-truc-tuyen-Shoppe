using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class EditProductViewModel
    {
        public string IdSanPham { get; set; }
        
        public string IdCuaHang { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public string IdDanhMuc { get; set; }
        
        public string TenDanhMuc { get; set; }
        
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string TenSanPham { get; set; }
        
        public string MoTa { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng kho phải là số dương")]
        public int? SoLuongKho { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải là số dương")]
        public decimal? GiaGoc { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải là số dương")]
        public decimal? GiaBan { get; set; }
        
        public string UrlAnh { get; set; }
        
        [Display(Name = "Ảnh mới (không bắt buộc)")]
        public IFormFile? NewUrlAnh { get; set; }
        
        public int SoLuongBan { get; set; }
    }
}