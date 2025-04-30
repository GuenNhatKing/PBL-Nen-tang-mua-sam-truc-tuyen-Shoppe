using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "danhmuc",
                columns: table => new
                {
                    IdDanhMuc = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenDanhMuc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdDanhMuc);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "nguoidung",
                columns: table => new
                {
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HoVaTen = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CCCD = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SDT = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiaChi = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VaiTro = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThoiGianXoa = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdNguoiDung);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "cuahang",
                columns: table => new
                {
                    IdCuaHang = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenCuaHang = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UrlAnh = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SDT = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiaChi = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThoiGianXoa = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdCuaHang);
                    table.ForeignKey(
                        name: "cuahang_ibfk_1",
                        column: x => x.IdNguoiDung,
                        principalTable: "nguoidung",
                        principalColumn: "IdNguoiDung");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "taikhoan",
                columns: table => new
                {
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Username = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdNguoiDung);
                    table.ForeignKey(
                        name: "taikhoan_ibfk_1",
                        column: x => x.IdNguoiDung,
                        principalTable: "nguoidung",
                        principalColumn: "IdNguoiDung");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "thongtinlienhe",
                columns: table => new
                {
                    IdLienHe = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HoVaTen = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SDT = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiaChi = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdLienHe);
                    table.ForeignKey(
                        name: "thongtinlienhe_ibfk_1",
                        column: x => x.IdNguoiDung,
                        principalTable: "nguoidung",
                        principalColumn: "IdNguoiDung");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "sanpham",
                columns: table => new
                {
                    IdSanPham = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdDanhMuc = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCuaHang = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TenSanPham = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UrlAnh = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoTa = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SoLuongKho = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TongDiemDanhGia = table.Column<int>(type: "int", nullable: false),
                    SoLuongDanhGia = table.Column<int>(type: "int", nullable: false),
                    SoLuongBan = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThoiGianXoa = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdSanPham);
                    table.ForeignKey(
                        name: "sanpham_ibfk_1",
                        column: x => x.IdDanhMuc,
                        principalTable: "danhmuc",
                        principalColumn: "IdDanhMuc");
                    table.ForeignKey(
                        name: "sanpham_ibfk_2",
                        column: x => x.IdCuaHang,
                        principalTable: "cuahang",
                        principalColumn: "IdCuaHang");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "donhang",
                columns: table => new
                {
                    IdDonHang = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdLienHe = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThoiGianGiao = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdDonHang);
                    table.ForeignKey(
                        name: "donhang_ibfk_1",
                        column: x => x.IdLienHe,
                        principalTable: "thongtinlienhe",
                        principalColumn: "IdLienHe");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "danhgia",
                columns: table => new
                {
                    IdDanhGia = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdSanPham = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiemDanhGia = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThoiGianDG = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdDanhGia);
                    table.ForeignKey(
                        name: "danhgia_ibfk_1",
                        column: x => x.IdNguoiDung,
                        principalTable: "nguoidung",
                        principalColumn: "IdNguoiDung");
                    table.ForeignKey(
                        name: "danhgia_ibfk_2",
                        column: x => x.IdSanPham,
                        principalTable: "sanpham",
                        principalColumn: "IdSanPham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "giohang",
                columns: table => new
                {
                    IdNguoiDung = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdSanPham = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.IdNguoiDung, x.IdSanPham })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "giohang_ibfk_1",
                        column: x => x.IdNguoiDung,
                        principalTable: "nguoidung",
                        principalColumn: "IdNguoiDung");
                    table.ForeignKey(
                        name: "giohang_ibfk_2",
                        column: x => x.IdSanPham,
                        principalTable: "sanpham",
                        principalColumn: "IdSanPham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "chitietdonhang",
                columns: table => new
                {
                    IdDonHang = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdSanPham = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.IdDonHang, x.IdSanPham })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "chitietdonhang_ibfk_1",
                        column: x => x.IdDonHang,
                        principalTable: "donhang",
                        principalColumn: "IdDonHang");
                    table.ForeignKey(
                        name: "chitietdonhang_ibfk_2",
                        column: x => x.IdSanPham,
                        principalTable: "sanpham",
                        principalColumn: "IdSanPham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IdSanPham",
                table: "chitietdonhang",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IdNguoiDung",
                table: "cuahang",
                column: "IdNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IdNguoiDung1",
                table: "danhgia",
                column: "IdNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IdSanPham1",
                table: "danhgia",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IdLienHe",
                table: "donhang",
                column: "IdLienHe");

            migrationBuilder.CreateIndex(
                name: "IdSanPham2",
                table: "giohang",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IdCuaHang",
                table: "sanpham",
                column: "IdCuaHang");

            migrationBuilder.CreateIndex(
                name: "IdDanhMuc",
                table: "sanpham",
                column: "IdDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IdNguoiDung2",
                table: "thongtinlienhe",
                column: "IdNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chitietdonhang");

            migrationBuilder.DropTable(
                name: "danhgia");

            migrationBuilder.DropTable(
                name: "giohang");

            migrationBuilder.DropTable(
                name: "taikhoan");

            migrationBuilder.DropTable(
                name: "donhang");

            migrationBuilder.DropTable(
                name: "sanpham");

            migrationBuilder.DropTable(
                name: "thongtinlienhe");

            migrationBuilder.DropTable(
                name: "danhmuc");

            migrationBuilder.DropTable(
                name: "cuahang");

            migrationBuilder.DropTable(
                name: "nguoidung");
        }
    }
}
