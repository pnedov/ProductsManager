using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ProductsManager.Models;

[ExcludeFromCodeCoverage]

[Table("suppliers")]
[Index(nameof(Name))]
public class Suppliers
{
    [Column("id", Order = 1)]
    [Key]
    public int Id { get; set; }

    [Column("iname", Order = 2, TypeName = "nvarchar(64)")]
    public string Name { get; set; } = string.Empty;

    [Column("add_date", Order = 3, TypeName = "datetime2")]
    [DefaultValue(typeof(DateTime), "0001-01-01")]
    public DateTime AddDate { get; set; } = DateTime.Now;

    [Column("upd_date", Order = 4, TypeName = "datetime2")]
    [DefaultValue(typeof(DateTime), "0001-01-01")]
    public DateTime UpdDate { get; set; }

    [Column("status", Order = 5, TypeName = "tinyint")]
    [DefaultValue(0)]
    public int Status { get; set; }

    [ForeignKey("SuppliersId")]
    public ICollection<WarehouseItem> WarehouseItem { get; set; } 
}

