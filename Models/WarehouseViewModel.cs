using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductsManager.Models;

public class WarehouseViewModel
{
    public string? SearchString { get; set; } = String.Empty;
    public string? Supplier { get; set; } = String.Empty;
    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }
    public List<WarehouseItem>? Items { get; set; } = new();
    public WarehouseItem? Item { get; set; } = new();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public List<Suppliers> Suppliers { get; set; } = new();
    public WarehouseItemFilters Filters { get; set; } = new();
    public decimal TotalPrice { get; set; } = new();
    public int TotalQuantity { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalProducts{ get; set; }
    public int TotalRecords { get; set; }
}

public struct WarehouseItemFilters
{
    public string? End { get; set; }
    public string? Start { get; set; }
    public int? SuppliersId { get; set; }
    public int? Status { get; set; }
    public string? SearchString { get; set; }
}

