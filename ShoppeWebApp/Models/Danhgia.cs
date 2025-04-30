using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

public partial class DanhGia
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string IdDanhGia { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdSanPham { get; set; } = null!;

    public int DiemDanhGia { get; set; }

    [StringLength(1000)]
    public string? NoiDung { get; set; }

    [Column("ThoiGianDG", TypeName = "datetime")]
    public DateTime ThoiGianDg { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("DanhGia")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    [InverseProperty("DanhGia")]
    public virtual SanPham IdSanPhamNavigation { get; set; } = null!;
}
