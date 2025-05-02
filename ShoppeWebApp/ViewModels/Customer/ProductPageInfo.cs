namespace ShoppeWebApp.ViewModels.Customer
{
    public class ProductPageInfo
    {
        public string IdDanhMuc { get; set; } = null!;
        public string IdCuaHang { get; set; } = null!;
        public string IdSanPham { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public string UrlAnh { get; set; } = null!;
        public string? MoTa { get; set; }
        public int SoLuongKho { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaBan { get; set; }
        public int TongDiemDanhGia { get; set; }
        public int SoLuongDanhGia { get; set; }
        public int SoLuongBan { get; set; }
        public string TenDanhMuc { get; set; } = null!;
        public string TenCuaHang { get; set; } = null!;
        public string UrlAnhCuaHang { get; set; } = null!;
        public int SoSanPhamDangBan { get; set; }
        public string SdtCuaHang { get; set; } = null!;
        public string DiaChiCuaHang { get; set; } = null!;
        public DateTime? ThoiGianThamGia { get; set; } = null!;
        public List<DanhGiaSanPham> DanhSachDanhGia = new List<DanhGiaSanPham>();
    }
    public class DanhGiaSanPham
    {
        public string IdNguoiDung { get; set; } = null!;
        public string? UrlAnhNguoiDung { get; set; } = null!;
        public string TenNguoiDung { get; set; } = null!;
        public int DiemDanhGia { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? ThoiGianDg { get; set; }

    }
}
