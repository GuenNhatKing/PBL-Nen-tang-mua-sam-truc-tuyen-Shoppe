using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("donhang")]
[Index("IdLienHe", Name = "IdLienHe")]
public partial class Donhang
{
    [Key]
    [StringLength(10)]
    public string IdDonHang { get; set; } = null!;

    [StringLength(10)]
    public string IdLienHe { get; set; } = null!;

    [Precision(18, 2)]
    public decimal TongTien { get; set; }

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianGiao { get; set; }

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    [ForeignKey("IdLienHe")]
    [InverseProperty("Donhangs")]
    public virtual Thongtinlienhe IdLienHeNavigation { get; set; } = null!;
}
