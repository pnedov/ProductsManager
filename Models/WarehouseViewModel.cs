using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductsManager.Models;

public class WarehouseViewModel
{
    public string? SearchString { get; set; }
    public string? Supplier { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public List<WarehouseItem>? Items { get; set; } = new();
    public WarehouseItem NewItem { get; set; } = new();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public List<Suppliers> Suppliers { get; set; } = new();
}

