using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("nguoidung")]
public partial class Nguoidung
{
    [Key]
    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoVaTen { get; set; } = null!;

    [Column("CCCD")]
    [StringLength(12)]
    public string Cccd { get; set; } = null!;

    [Column("SDT")]
    [StringLength(10)]
    public string Sdt { get; set; } = null!;

    [StringLength(100)]
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
    public virtual ICollection<Cuahang> Cuahangs { get; set; } = new List<Cuahang>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<Danhgia> Danhgia { get; set; } = new List<Danhgia>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual Taikhoan? Taikhoan { get; set; }

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<Thongtinlienhe> Thongtinlienhes { get; set; } = new List<Thongtinlienhe>();
}
