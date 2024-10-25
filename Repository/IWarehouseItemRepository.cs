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
    Task AddItemAsync(WarehouseItem item, CancellationToken token);
    Task UpdateItemAsync(WarehouseItem item, CancellationToken token);
    Task DeleteItemAsync(int id, CancellationToken token);
}

