using ProductsManager.Models;
using Microsoft.EntityFrameworkCore;
using ProductsManager.Services;
using ProductsManager.Repository;
using FluentValidation;
using ProductsManager.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var configuration = builder.Configuration;
builder.Services.AddScoped<WarehouseDbContext>();
builder.Services.AddScoped<IWarehouseItemRepository,WarehouseItemRepository>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddTransient<IValidator<GetWarehouseItemRequest>, GetWarehouseItemRequestValidator>();
builder.Services.AddAutoMapper(typeof(ModelsMapper).Assembly);

var app = builder.Build();

//var configuration = builder.Configuration;
var services = builder.Services;
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
dbContext.Database.Migrate();
dbContext.Database.EnsureCreated();

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
