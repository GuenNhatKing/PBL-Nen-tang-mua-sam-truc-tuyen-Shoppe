using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("thongtinlienhe")]
[Index("IdNguoiDung", Name = "IdNguoiDung")]
public partial class Thongtinlienhe
{
    [Key]
    [StringLength(10)]
    public string IdLienHe { get; set; } = null!;

    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string HoVaTen { get; set; } = null!;

    [Column("SDT")]
    [StringLength(10)]
    public string Sdt { get; set; } = null!;

    [StringLength(1000)]
    public string DiaChi { get; set; } = null!;
    public bool? DaXoa { get; set; } = null!;

    [InverseProperty("IdLienHeNavigation")]
    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Thongtinlienhes")]
    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;
}
