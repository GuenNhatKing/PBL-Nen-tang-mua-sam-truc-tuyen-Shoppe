using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ThemAnhDaiDien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlAnh",
                table: "nguoidung",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlAnh",
                table: "nguoidung");
        }
    }
}
