using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services;

using Microsoft.AspNetCore.Identity;
using BCrypt.Net;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<StaffRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<MenuItemRepository>();
builder.Services.AddScoped<StockRepository>();
builder.Services.AddScoped<MenuRepository>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<TableRepository>();


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
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Bar_Kitchen",
    pattern: "Bar/{action=Index}/{id?}",
    defaults: new { controller = "Bar_Kitchen" });    

app.Run();
