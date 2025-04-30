using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[PrimaryKey("IdDonHang", "IdSanPham")]
[Table("chitietdonhang")]
[Index("IdSanPham", Name = "IdSanPham")]
public partial class Chitietdonhang
{
    [Key]
    [StringLength(10)]
    public string IdDonHang { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [Precision(18, 2)]
    public decimal DonGia { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("Chitietdonhangs")]
    public virtual Donhang IdDonHangNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    [InverseProperty("Chitietdonhangs")]
    public virtual Sanpham IdSanPhamNavigation { get; set; } = null!;
}
