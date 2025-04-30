using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;

namespace ShoppeWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine(builder.Configuration.GetConnectionString("ShoppeWebApp"));
            builder.Services.AddDbContext<ShoppeWebAppDbContext>(options =>
            {
                options.UseMySql(builder.Configuration.GetConnectionString("ShoppeWebApp"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ShoppeWebApp")));
            });
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
