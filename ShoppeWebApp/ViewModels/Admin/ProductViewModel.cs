using System.Collections.Generic;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class ProductViewModel
    {
        // Danh sách sản phẩm
        public List<ProductInfo> productInfos { get; set; } = new List<ProductInfo>();

        // Danh sách danh mục
        public List<CategoryInfo> categories { get; set; } = new List<CategoryInfo>();

        // ID danh mục hiện tại 
        public string? danhMuc { get; set; }

        // Thông tin cửa hàng
        public string? IdCuaHang { get; set; }
        public string? TenCuaHang { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? UrlAnhCuaHang { get; set; }
    }

    // Thông tin sản phẩm
    public class ProductInfo
    {
        public string? IdSanPham { get; set; } // ID sản phẩm
        public string? TenSanPham { get; set; } // Tên sản phẩm
        public decimal GiaGoc { get; set; } // Giá gốc
        public decimal GiaBan { get; set; } // Giá bán
        public int SoLuongBan { get; set; } // Số lượng đã bán
        public string? UrlAnh { get; set; } // URL ảnh sản phẩm
        public int TrangThai { get; set; }
        public decimal TyLeGiamGia { get; set; } // Tỷ lệ giảm giá

        // Thuộc tính chỉ đọc để định dạng số lượng đã bán
        public string SoLuongDaBan => ProcessQuantity(SoLuongBan);

        // Phương thức định dạng số lượng
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

    // Thông tin danh mục
    public class CategoryInfo
    {
        public string? IdDanhMuc { get; set; } // ID danh mục
        public string? TenDanhMuc { get; set; } // Tên danh mục
    }
}