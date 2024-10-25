
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProductsManager.Models;

public class WarehouseDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public WarehouseDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("WarehouseDb"), option =>
        {
            option.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        });

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WarehouseItem>(e =>
        {
            e.HasKey(k => k.Id);
        });

        base.OnModelCreating(modelBuilder);
    }


    public DbSet<WarehouseItem> WarehouseItems { get; set; }
    public DbSet<Suppliers> Suppliers { get; set; }
}

