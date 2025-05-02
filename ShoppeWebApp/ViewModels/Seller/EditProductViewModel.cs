namespace ShoppeWebApp.ViewModels.Seller
{
    public class EditProductViewModel
    {
        public string IdSanPham { get; set; }
        public string TenSanPham { get; set; }
        public decimal GiaBan { get; set; }
        public string MoTa { get; set; }
        public string UrlAnhHienTai { get; set; }
        public IFormFile? UrlAnhMoi { get; set; }
        public int SoLuongKho { get; set; }
        public string IdDanhMuc { get; set; }
    }
}