using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("sanpham")]
[Index("IdCuaHang", Name = "IdCuaHang")]
[Index("IdDanhMuc", Name = "IdDanhMuc")]
public partial class Sanpham
{
    [Key]
    [StringLength(10)]
    public string IdSanPham { get; set; } = null!;

    [StringLength(10)]
    public string IdDanhMuc { get; set; } = null!;

    [StringLength(10)]
    public string IdCuaHang { get; set; } = null!;

    [StringLength(100)]
    public string TenSanPham { get; set; } = null!;

    [StringLength(2048)]
    public string UrlAnh { get; set; } = null!;

    [Column(TypeName ="longtext")]
    public string? MoTa { get; set; }

    public int SoLuongKho { get; set; }

    public int TrangThai { get; set; }

    [Precision(18, 2)]
    public decimal GiaGoc { get; set; }

    [Precision(18, 2)]
    public decimal GiaBan { get; set; }

    public int TongDiemDanhGia { get; set; }

    public int SoLuongDanhGia { get; set; }

    public int SoLuongBan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianXoa { get; set; }

    [InverseProperty("IdSanPhamNavigation")]
    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    [InverseProperty("IdSanPhamNavigation")]
    public virtual ICollection<Danhgia> Danhgia { get; set; } = new List<Danhgia>();

    [InverseProperty("IdSanPhamNavigation")]
    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    [ForeignKey("IdCuaHang")]
    [InverseProperty("Sanphams")]
    public virtual Cuahang IdCuaHangNavigation { get; set; } = null!;

    [ForeignKey("IdDanhMuc")]
    [InverseProperty("Sanphams")]
    public virtual Danhmuc IdDanhMucNavigation { get; set; } = null!;
}
