using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.Data;

public partial class ShoppeWebAppDbContext : DbContext
{
    public ShoppeWebAppDbContext(DbContextOptions<ShoppeWebAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chitietdonhang> Chitietdonhangs { get; set; }

    public virtual DbSet<Cuahang> Cuahangs { get; set; }

    public virtual DbSet<Danhgia> Danhgia { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<Giohang> Giohangs { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thongtinlienhe> Thongtinlienhes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Chitietdonhang>(entity =>
        {
            entity.HasKey(e => new { e.IdDonHang, e.IdSanPham })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.Chitietdonhangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chitietdonhang_ibfk_1");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.Chitietdonhangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chitietdonhang_ibfk_2");
        });

        modelBuilder.Entity<Cuahang>(entity =>
        {
            entity.HasKey(e => e.IdCuaHang).HasName("PRIMARY");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.Cuahangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuahang_ibfk_1");
        });

        modelBuilder.Entity<Danhgia>(entity =>
        {
            entity.HasKey(e => e.IdDanhGia).HasName("PRIMARY");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("danhgia_ibfk_1");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("danhgia_ibfk_2");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.IdDanhMuc).HasName("PRIMARY");
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasKey(e => e.IdDonHang).HasName("PRIMARY");

            entity.HasOne(d => d.IdLienHeNavigation).WithMany(p => p.Donhangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("donhang_ibfk_1");
        });

        modelBuilder.Entity<Giohang>(entity =>
        {
            entity.HasKey(e => new { e.IdNguoiDung, e.IdSanPham })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.Giohangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("giohang_ibfk_1");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.Giohangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("giohang_ibfk_2");
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.IdNguoiDung).HasName("PRIMARY");
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.IdSanPham).HasName("PRIMARY");

            entity.HasOne(d => d.IdCuaHangNavigation).WithMany(p => p.Sanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sanpham_ibfk_2");

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.Sanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sanpham_ibfk_1");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.IdNguoiDung).HasName("PRIMARY");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithOne(p => p.Taikhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("taikhoan_ibfk_1");
        });

        modelBuilder.Entity<Thongtinlienhe>(entity =>
        {
            entity.HasKey(e => e.IdLienHe).HasName("PRIMARY");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.Thongtinlienhes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("thongtinlienhe_ibfk_1");
            entity.HasQueryFilter(i => i.DaXoa != true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
