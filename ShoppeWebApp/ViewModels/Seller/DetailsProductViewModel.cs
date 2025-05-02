using System;
using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class DetailsProductViewModel
    {
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
        public List<DanhGiaInfo> DanhGias { get; set; } = new List<DanhGiaInfo>();

        public class DanhGiaInfo
        {
            public string? IdDanhGia { get; set; }
            public string? IdNguoiDung { get; set; }
            public string? TenNguoiDung { get; set; }   
            public int DiemDanhGia { get; set; }
            public string? NoiDung { get; set; }
            public DateTime? ThoiGianDG { get; set; }
        }
    }
}
