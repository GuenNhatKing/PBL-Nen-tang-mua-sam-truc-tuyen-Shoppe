using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("ThanhToan")]
public partial class ThanhToan
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdThanhToan { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdDonHang { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTien { get; set; }

    public int LoaiThanhToan { get; set; }

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ThoiGianTao { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("ThanhToans")]
    public virtual DonHang IdDonHangNavigation { get; set; } = null!;
}
