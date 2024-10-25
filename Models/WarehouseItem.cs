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

    [Required]
    [Column("iname", Order = 2, TypeName = "nvarchar(64)")]
    public string Name { get; set; }

    [Required]
    [Column("unique_code", Order = 3, TypeName = "nvarchar(64)")]
    public string UniqueCode { get; set; }

    [Required]
    [Column("quantity", Order = 3, TypeName = "int")]
    public int Quantity { get; set; }

    [Required]
    [Column("price", Order = 4, TypeName = "decimal(19, 4)")]
    public decimal Price { get; set; }

    [Column("status", Order = 5, TypeName = "tinyint")]
    [DefaultValue(0)]
    public int Status { get; set; } = 0;

    [Column("add_date", Order = 6, TypeName = "datetime2")]
    [DefaultValue(typeof(DateTime), "0001-01-01")]
    public DateTime AddDate { get; set; } = DateTime.Now;

    [Column("upd_date", Order = 7, TypeName = "datetime2")]
    [DefaultValue(typeof(DateTime), "0001-01-01")]
    public DateTime UpdDate { get; set; }

    [Column("suppliers_id", Order = 8, TypeName = "int")]
    public int SuppliersId { get; set; }

    public Suppliers Suppliers { get; set; } = new();
}

