using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppeWebApp.ViewModels.Admin
{
    public class AllReviewsViewModel
    {
        public string? IdSanPham { get; set; }
        public string? IdCuaHang { get; set; }
        public string? FilterByStars { get; set; }
        public List<ReviewViewModel>? DanhSachDanhGia { get; set; } = new List<ReviewViewModel>();
        public class ReviewViewModel
        {
           public string? IdDanhGia { get; set; }

            public string? IdNguoiDung { get; set; }

            public string? TenNguoiDung { get; set; }

            public int DiemDanhGia { get; set; }

            public string? NoiDung { get; set; }

            public DateTime? ThoiGianDG { get; set; }
            public string ThoiGianDGFormatted => ThoiGianDG.HasValue
                ? ThoiGianDG.Value.ToString("dd/MM/yyyy HH:mm")
                : "N/A";
        }
    }
}