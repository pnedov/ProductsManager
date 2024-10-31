using ProductsManager.Models;
using Microsoft.EntityFrameworkCore;
using ProductsManager.Services;
using ProductsManager.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var configuration = builder.Configuration;
builder.Services.AddScoped<WarehouseDbContext>();
builder.Services.AddScoped<IWarehouseItemRepository,WarehouseItemRepository>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddAutoMapper(typeof(ModelsMapper).Assembly);

var app = builder.Build();
var services = builder.Services;
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
dbContext.Database.Migrate();
dbContext.Database.EnsureCreated();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Warehouse}/{action=Index}/{id?}");

app.Run();
