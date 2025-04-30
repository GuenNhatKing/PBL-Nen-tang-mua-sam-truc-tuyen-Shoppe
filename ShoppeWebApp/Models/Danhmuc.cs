using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("DanhMuc")]
public partial class DanhMuc
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdDanhMuc { get; set; } = null!;

    [StringLength(50)]
    public string TenDanhMuc { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("IdDanhMucNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
