using ProductsManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProductsManager.Services;
using ProductsManager.Controllers;
using ProductsManager.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext with the dependency injection container
//builder.Services.AddDbContext<WarehouseContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var configuration = builder.Configuration;
builder.Services.AddScoped<WarehouseDbContext>();
builder.Services.AddScoped<IWarehouseItemRepository,WarehouseItemRepository>();
builder.Services.AddScoped<ISystemService, SystemService>();

var app = builder.Build();

//var configuration = builder.Configuration;
var services = builder.Services;
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
dbContext.Database.Migrate();
dbContext.Database.EnsureCreated();
//var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
//if (!dbCreator.EnsureCreated())
//{
//    dbCreator.CreateTables();
//}
//
//if (!dbContext.WarehouseItems.AnyAsync().Result )
//{
//    var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
//    dbCreator.CreateTables();
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Warehouse}/{action=Index}/{id?}");

app.Run();
