using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoppeWebApp.Models;

[Table("TaiKhoan")]
public partial class TaiKhoan
{
    [StringLength(10)]
    [Unicode(false)]
    public string IdNguoiDung { get; set; } = null!;

    [Key]
    [StringLength(30)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(60)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("TaiKhoans")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;
}


