using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("taikhoan")]
public partial class Taikhoan
{
    [Key]
    [StringLength(10)]
    public string IdNguoiDung { get; set; } = null!;

    [StringLength(30)]
    public string Username { get; set; } = null!;

    [StringLength(60)]
    public string Password { get; set; } = null!;

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Taikhoan")]
    public virtual Nguoidung IdNguoiDungNavigation { get; set; } = null!;
}
