using System;
using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Seller
{
    public class ProductViewModel
    {
        public List<ProductInfo> Products { get; set; } = new List<ProductInfo>();
        public List<CategoryInfo> Categories { get; set; } = new List<CategoryInfo>();

        public string? danhMuc { get; set; }

        public string? IdCuaHang { get; set; }
        public string? TenCuaHang { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? UrlAnhCuaHang { get; set; }
    }

    public class ProductInfo
    {
        public string? IdSanPham { get; set; } 
        public string? TenSanPham { get; set; } 
        public decimal GiaGoc { get; set; } 
        public decimal GiaBan { get; set; } 
        public int SoLuongBan { get; set; } 
        public string? UrlAnh { get; set; }
        public decimal TyLeGiamGia { get; set; } 
        public int TrangThai { get; set; }
        public DateTime? ThoiGianXoa { get; set; }

        public string SoLuongDaBan => ProcessQuantity(SoLuongBan);

        private static string ProcessQuantity(int quantity)
        {
            double curr = quantity;
            int expo = 0;
            string[] symbol = { "", "K", "M", "B", "T", "Q", "Qi" };
            while (curr >= 1000)
            {
                curr /= 1000;
                ++expo;
                if (expo >= symbol.Length) break;
            }
            string res = string.Format("{0:0.#}{1}", curr, symbol[expo]);
            return res;
        }
    }

    public class CategoryInfo
    {
        public string? IdDanhMuc { get; set; } 
        public string? TenDanhMuc { get; set; } 
    }
}