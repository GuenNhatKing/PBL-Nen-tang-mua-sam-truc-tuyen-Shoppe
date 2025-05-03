using System;
using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class DetailsProductViewModel
    {
        // Thông tin sản phẩm
        public string? IdSanPham { get; set; }
        public string? IdCuaHang { get; set; }
        public string? TenSanPham { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? UrlAnh { get; set; }
        public string? MoTa { get; set; }
        public int SoLuongKho { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaBan { get; set; }
        public int TrangThai { get; set; }
        public int TongDiemDG { get; set; }
        public int SoLuotDG { get; set; }
        public int SoLuongBan { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public DateTime? ThoiGianXoa { get; set; }
    }
}
