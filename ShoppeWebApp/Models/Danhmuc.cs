using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("danhmuc")]
public partial class Danhmuc
{
    [Key]
    [StringLength(10)]
    public string IdDanhMuc { get; set; } = null!;

    [StringLength(100)]
    public string TenDanhMuc { get; set; } = null!;

    [InverseProperty("IdDanhMucNavigation")]
    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
