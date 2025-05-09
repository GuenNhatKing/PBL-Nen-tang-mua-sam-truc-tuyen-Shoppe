using System;
using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class OrderDetailsViewModel
    {
        public string MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public ThongTinLienHeViewModel ThongTinLienHe { get; set; }
        public List<SanPhamViewModel> SanPham { get; set; }
    }

    public class ThongTinLienHeViewModel
    {
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class SanPhamViewModel
    {
        public string IdSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string UrlAnh { get; set; } 
        public List<DanhGiaViewModel> DanhGia { get; set; } 
    }

    public class DanhGiaViewModel
    {
        public string TenNguoiDung { get; set; }
        public int DiemDanhGia { get; set; } 
        public string NoiDung { get; set; } 
        public string ThoiGianDG { get; set; } 
    }
}