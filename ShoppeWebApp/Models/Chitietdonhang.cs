using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[PrimaryKey("IdDonHang", "IdSanPham")]
[Table("ChiTietDonHang")]
public partial class ChiTietDonHang
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdDonHang { get; set; } = null!;

    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DonGia { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual DonHang IdDonHangNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual SanPham IdSanPhamNavigation { get; set; } = null!;
}
