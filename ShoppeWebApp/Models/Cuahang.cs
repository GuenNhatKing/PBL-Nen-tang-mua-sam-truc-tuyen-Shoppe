using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("cuahang")]
[Index("IdNguoiDung", Name = "IdNguoiDung")]
public partial class Cuahang
{
    [Key]
    [StringLength(10)]
    public string IdCuaHang { get; set; } = null!;

    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(100)]
    public string TenCuaHang { get; set; } = null!;

    [StringLength(100)]
    public string UrlAnh { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [Column("SDT")]
    [StringLength(10)]
    public string Sdt { get; set; } = null!;

    [StringLength(1000)]
    public string DiaChi { get; set; } = null!;

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianXoa { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Cuahangs")]
    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;

    [InverseProperty("IdCuaHangNavigation")]
    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
