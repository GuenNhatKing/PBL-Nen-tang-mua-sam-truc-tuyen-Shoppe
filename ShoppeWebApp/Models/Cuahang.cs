using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("CuaHang")]
public partial class CuaHang
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdCuaHang { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(100)]
    public string TenCuaHang { get; set; } = null!;

    [StringLength(100)]
    public string UrlAnh { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [Column("SDT")]
    [StringLength(10)]
    [Unicode(false)]
    public string Sdt { get; set; } = null!;

    [StringLength(255)]
    public string DiaChi { get; set; } = null!;

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianXoa { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("CuaHangs")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;

    [InverseProperty("IdCuaHangNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
