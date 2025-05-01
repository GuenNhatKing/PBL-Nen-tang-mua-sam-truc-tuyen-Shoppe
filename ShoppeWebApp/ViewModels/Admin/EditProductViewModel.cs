using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class ChinhSuaSanPhamViewModel
    {
        public string? IdCuaHang { get; set; }
        public string? IdSanPham { get; set; }

        // Tên sản phẩm
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string? TenSanPham { get; set; }

        // Giá gốc
        [Required(ErrorMessage = "Giá gốc là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn hoặc bằng 0.")]
        public decimal GiaGoc { get; set; }

        // Giá bán
        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        public decimal GiaBan { get; set; }

        // Số lượng trong kho
        [Required(ErrorMessage = "Số lượng kho là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng kho phải lớn hơn hoặc bằng 0.")]
        public int SoLuongKho { get; set; }

        // URL ảnh sản phẩm (nếu có)
        public string? DuongDanAnh { get; set; }

        // Ảnh sản phẩm mới (upload)
        public IFormFile? AnhMoi { get; set; }

        // ID danh mục hiện tại
        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public string? MaDanhMucDuocChon { get; set; }

        // Danh sách danh mục để chọn
        public List<ThongTinDanhMuc> DanhSachDanhMuc { get; set; } = new List<ThongTinDanhMuc>();

        public string? MoTa { get; set; }
    }

    public class ThongTinDanhMuc
    {
        public string? MaDanhMuc { get; set; } // ID danh mục
        public string? TenDanhMuc { get; set; } // Tên danh mục
    }
}