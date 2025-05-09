using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppeWebApp.ViewModels.Customer
{
    public class ManageProducts
    {
        [ValidateNever]
        public string IdNguoiDung { get; set; } = null!;
        [ValidateNever]
        public string? UrlAnhDaiDien { get; set; }
        public string HoVaTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<ManageProductOrder> danhSachDonHang = new List<ManageProductOrder>();
    }
    public class ManageProductOrder
    {
        public string IdDonHang { get; set; } = null!;
        public int TinhTrang { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public DateTime? ThoiGianGiao { get; set; }
        public List<OrderDescCuaHang> danhSachCuaHang = new List<OrderDescCuaHang>();
        public decimal TongTien { get; set; }
    }
    public class OrderDescCuaHang
    {
        public string IdCuaHang { get; set; } = null!;
        public string TenCuaHang { get; set; } = null!;
        public List<OrderDescSanPham> danhSachSanPhams = new List<OrderDescSanPham>();
    }
    public class OrderDescSanPham
    {
        public string IdSanPham { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public string UrlAnhSanPham { get; set; } = null!;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        
        public List<DanhGiaInfo> danhGias = new List<DanhGiaInfo>();
    }

    public class DanhGiaInfo
    {
        public string IdDanhGia { get; set; } = null!;
        public string? NoiDungDanhGia { get; set; } = null!;
        public DateTime? ThoiGianDanhGia { get; set; }
        public int SoSaoDanhGia { get; set; }
    }
}
