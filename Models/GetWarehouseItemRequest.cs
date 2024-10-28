namespace ProductsManager.Models;

public class GetWarehouseItemRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string UniqueCode { get; set; } = string.Empty;
    public int SuppliersId { get; set; }
    public int Status { get; set; }
}
