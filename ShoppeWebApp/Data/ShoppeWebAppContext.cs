using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Models;

namespace ShoppeWebApp.Data;

public partial class ShoppeWebAppContext : DbContext
{
    public ShoppeWebAppContext(DbContextOptions<ShoppeWebAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<CuaHang> CuaHangs { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<ThongTinLienHe> ThongTinLienHes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_DonHang");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_SanPham");
        });

        modelBuilder.Entity<CuaHang>(entity =>
        {
            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.CuaHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CuaHang_NguoiDung");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.DanhGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGia_NguoiDung");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.DanhGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGia_SanPham");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasOne(d => d.IdLienHeNavigation).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_ThongTinLienHe");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_NguoiDung");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_SanPham");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasOne(d => d.IdCuaHangNavigation).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_CuaHang");

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_DanhMuc");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.TaiKhoans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_NguoiDung");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ThanhToans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThanhToan_DonHang");
        });

        modelBuilder.Entity<ThongTinLienHe>(entity =>
        {
            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.ThongTinLienHes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThongTinLienHe_NguoiDung");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
