using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[PrimaryKey("IdNguoiDung", "IdSanPham")]
[Table("giohang")]
[Index("IdSanPham", Name = "IdSanPham")]
public partial class Giohang
{
    [Key]
    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Giohangs")]
    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    [InverseProperty("Giohangs")]
    public virtual Sanpham IdSanPhamNavigation { get; set; } = null!;
}
