﻿using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ShoppeWebApp.Data;
using Hangfire;
using Hangfire.MySql;
using ShoppeWebApp.Services;
using Microsoft.AspNetCore.Identity;

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
            var storageOptions = new MySqlStorageOptions();
            builder.Services.AddHangfire(config =>
            {
                config.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("ShoppeWebApp"), storageOptions));
            });
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<DatabaseChecker>();

            builder.Services.AddAuthentication("CustomerSchema")
            .AddCookie("CustomerSchema", options =>
            {
                options.LoginPath = "/Customer/Account/Login";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
                options.Cookie.Name = "CustomerCookie";
            })
            .AddCookie("SellerSchema", options =>
             {
                 options.LoginPath = "/Seller/Account/Login";
                 options.AccessDeniedPath = "/Authentication/AccessDenied";
                 options.Cookie.Name = "SellerCookie";
             });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                options.AddPolicy("Customer", policy => policy.RequireClaim(ClaimTypes.Role, "Customer"));
                options.AddPolicy("Seller", policy => policy.RequireClaim(ClaimTypes.Role, "Seller"));
            });
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<DatabaseChecker>(
                "check-db-every-5-minutes",
                checker => checker.CheckDatabase(),
                "* * * * *"
            );
            // Hien tai la 1 phut cho de check

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
