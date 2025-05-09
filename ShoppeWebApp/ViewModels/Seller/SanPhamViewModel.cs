using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class SanPhamSellerViewModel
    {
        public string IdSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string UrlAnh { get; set; }
        public List<DanhGiaViewModel> DanhGia { get; set; }
    }
}
