using Microsoft.EntityFrameworkCore;
using ProductsManager.Models;

namespace ProductsManager.Repository;

public class WarehouseItemRepository : IWarehouseItemRepository
{
    private readonly WarehouseDbContext _context;

    public WarehouseItemRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(CancellationToken token)
    {
        return await _context.WarehouseItems.ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(int supplierId, CancellationToken token)
    {
        return await _context.WarehouseItems.Where(x => x.SuppliersId == supplierId).ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(int minQuantity, int maxQuantity, CancellationToken token)
    {
        return await _context.WarehouseItems.Where(x => x.Quantity >= minQuantity && x.Quantity <= maxQuantity).ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(int supplierId, int minQuantity, int maxQuantity, CancellationToken token)
    {
        return await _context.WarehouseItems.Where(x => x.SuppliersId == supplierId && x.Quantity >= minQuantity && x.Quantity <= maxQuantity).ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(WarehouseItem item, CancellationToken token)
    {
        return await _context.WarehouseItems.Where(x => x.SuppliersId == item.SuppliersId && x.Quantity >= item.Quantity
        && x.Quantity <= item.Quantity).ToListAsync(token);
    }

    public async Task<WarehouseItem?> GetSingleItemAsync(int id, CancellationToken token)
    {
        return await _context.WarehouseItems.FirstOrDefaultAsync(x => x.Id == id, token);
    }

    public async Task AddItemAsync(WarehouseItem item, CancellationToken token)
    {
        _context.WarehouseItems.Add(item);
        await _context.SaveChangesAsync(token);
    }

    public async Task UpdateItemAsync(WarehouseItem item, CancellationToken token)
    {
        _context.WarehouseItems.Update(item);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteItemAsync(int id, CancellationToken token)
    {
        var item = await this.GetSingleItemAsync(id, token);
        _context.WarehouseItems.Remove(item);
        await _context.SaveChangesAsync(token);
    }
}

