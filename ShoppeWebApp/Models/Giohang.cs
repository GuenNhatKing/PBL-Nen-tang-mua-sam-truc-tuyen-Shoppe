using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Keyless]
[Table("GioHang")]
public partial class GioHang
{
    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string IdSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    [ForeignKey("IdNguoiDung")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdSanPham")]
    public virtual SanPham IdSanPhamNavigation { get; set; } = null!;
}
