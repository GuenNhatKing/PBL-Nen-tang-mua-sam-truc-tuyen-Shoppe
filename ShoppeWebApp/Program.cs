using Microsoft.AspNetCore.Routing;
using ShoppeWebApp.Constraints;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; 

namespace ShoppeWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            // Sử dụng MySQL thay vì SQL Server
            builder.Services.AddDbContext<ShoppeWebAppContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("ShoppeWebApp"),
                    new MySqlServerVersion(new Version(8, 0, 33)) // Thay đổi phiên bản MySQL của bạn tại đây
             ));
            // Đăng ký ràng buộc Admin
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("Admin", typeof(AdminConstraint));
            });

            // Thêm cấu hình cho CustomerSchema
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "CustomerSchema"; // Đặt mặc định là CustomerSchema
            })
            .AddCookie("CustomerSchema", options =>
            {
                options.LoginPath = "/Customer/Account/Login"; // Đường dẫn đến trang đăng nhập của Customer
                options.AccessDeniedPath = "/Authentication/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
            })
            .AddCookie("SellerSchema", options =>
            {
                options.LoginPath = "/Seller/Account/Login"; // Đường dẫn đến trang đăng nhập của Seller
                options.AccessDeniedPath = "/Seller/Account/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
            });

            // Thêm chính sách Admin vào cấu hình Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin"); // Yêu cầu người dùng phải có vai trò Admin
                });
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Route cho tất cả các areas
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}