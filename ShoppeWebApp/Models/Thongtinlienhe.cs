using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("ThongTinLienHe")]
public partial class ThongTinLienHe
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdLienHe { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoVaTen { get; set; } = null!;

    [Column("SDT")]
    [StringLength(10)]
    [Unicode(false)]
    public string Sdt { get; set; } = null!;

    [StringLength(255)]
    public string DiaChi { get; set; } = null!;

    [InverseProperty("IdLienHeNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("ThongTinLienHes")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;
}
