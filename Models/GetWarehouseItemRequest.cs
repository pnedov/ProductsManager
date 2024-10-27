namespace ProductsManager.Models;

public class GetWarehouseItemRequest
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string UniqueCode { get; set; }
    public int SuppliersId { get; set; }
}

