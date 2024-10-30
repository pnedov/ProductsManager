using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ProductsManager.Models;

[Table("warehouse_items")]
[Index(nameof(Name), nameof(UniqueCode))]
[Index(nameof(UniqueCode), IsUnique = true)]
public class WarehouseItem
{
    [Column("id", Order = 1)]
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [Column("iname", Order = 2, TypeName = "nvarchar(64)")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unique Code is required.")]
    [Column("unique_code", Order = 3, TypeName = "nvarchar(64)")]
    public string UniqueCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required.")]
    [Column("quantity", Order = 4, TypeName = "int")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 999.99)]
    [Column("price", Order = 4, TypeName = "decimal(19, 4)")]
    public decimal Price { get; set; }

    [Column("status", Order = 5, TypeName = "tinyint")]
    [DefaultValue(0)]
    public int Status { get; set; } = 0;

    [Column("add_date", Order = 6, TypeName = "datetime2")]
    [DefaultValue(typeof(DateTime), "0001-01-01")]
    public DateTime AddDate { get; set; } = DateTime.Now;

    [Column("upd_date", Order = 7, TypeName = "datetime2")]
    public DateTime UpdDate { get; set; }

    [Required(ErrorMessage = "Supplier is required.")]
    [Column("suppliers_id", Order = 8, TypeName = "int")]
    public int SuppliersId { get; set; }

    public Suppliers Suppliers { get; set; } = new();
}

// list of item's statuses
public enum Statuses
{
    Active = 0,
    Sold = 1,
    Paused = 2
}

