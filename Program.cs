using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBar_KitchenRepository, Bar_KitchenRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<FinancialRepository>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBar_KitchenService, Bar_KitchenService>();  
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<FinancialService>();
// Schakel authenticatie in met het standaard �Cookies� schema
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(opts =>
        {
    opts.LoginPath = "/Login/Index";
    opts.AccessDeniedPath = "/Home/AccessDenied";
        });


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Bar_Kitchen",
    pattern: "Bar/{action=Index}/{id?}",
    defaults: new { controller = "Bar_Kitchen" });    

app.Run();
