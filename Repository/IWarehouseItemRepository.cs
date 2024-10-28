using ProductsManager.Models;

namespace ProductsManager.Repository;

public interface IWarehouseItemRepository
{
    Task<List<WarehouseItem>> GetItemsAsync(CancellationToken token);
    Task<List<WarehouseItem>> GetItemsAsync(int supplierId, CancellationToken token);
    Task<List<WarehouseItem>> GetItemsAsync(int minQuantity, int maxQuantity, CancellationToken token);
    Task<List<WarehouseItem>> GetItemsAsync(int supplierId, int minQuantity, int maxQuantity, CancellationToken token);
    Task<List<WarehouseItem>> GetItemsAsync(WarehouseItem item, CancellationToken token);
    Task<WarehouseItem?> GetSingleItemAsync(int itemId, CancellationToken token);
    Task<List<Suppliers>> GetSuppliersAsync(CancellationToken token);
    Task AddItemAsync(GetWarehouseItemRequest item, CancellationToken token);
    Task UpdateItemAsync(GetWarehouseItemRequest item, WarehouseItem oldItem, CancellationToken token);
    Task DeleteItemAsync(int id, CancellationToken token);
    Task DeleteMultiItemsAsync(string item, CancellationToken token);
}

