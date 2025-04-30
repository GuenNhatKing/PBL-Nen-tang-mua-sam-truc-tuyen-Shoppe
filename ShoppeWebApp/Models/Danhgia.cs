using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("danhgia")]
[Index("IdNguoiDung", Name = "IdNguoiDung")]
[Index("IdSanPham", Name = "IdSanPham")]
public partial class Danhgia
{
    [Key]
    [StringLength(10)]
    public string IdDanhGia { get; set; } = null!;

    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(10)]
    public string IdSanPham { get; set; } = null!;

    public int DiemDanhGia { get; set; }

    [StringLength(1000)]
    public string? NoiDung { get; set; }

    [Column("ThoiGianDG", TypeName = "datetime")]
    public DateTime? ThoiGianDg { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Danhgia")]
    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    [InverseProperty("Danhgia")]
    public virtual Sanpham IdSanPhamNavigation { get; set; } = null!;
}
