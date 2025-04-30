using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("NguoiDung")]
public partial class NguoiDung
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoVaTen { get; set; } = null!;

    [Column("CCCD")]
    [StringLength(12)]
    [Unicode(false)]
    public string Cccd { get; set; } = null!;

    [Column("SDT")]
    [StringLength(10)]
    [Unicode(false)]
    public string Sdt { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(1000)]
    public string DiaChi { get; set; } = null!;

    public int VaiTro { get; set; }

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianXoa { get; set; }

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<CuaHang> CuaHangs { get; set; } = new List<CuaHang>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<ThongTinLienHe> ThongTinLienHes { get; set; } = new List<ThongTinLienHe>();
}
