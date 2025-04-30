using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("DonHang")]
public partial class DonHang
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdDonHang { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdLienHe { get; set; } = null!;

    public int TongTien { get; set; }

    public int TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ThoiGianTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianGiao { get; set; }

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [ForeignKey("IdLienHe")]
    [InverseProperty("DonHangs")]
    public virtual ThongTinLienHe IdLienHeNavigation { get; set; } = null!;

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
