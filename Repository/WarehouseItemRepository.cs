﻿using Microsoft.EntityFrameworkCore;
using ProductsManager.Models;
using AutoMapper;

namespace ProductsManager.Repository;

public class WarehouseItemRepository : IWarehouseItemRepository
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;

    public WarehouseItemRepository(IMapper mapper, WarehouseDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WarehouseItem>> GetItemsAsync(CancellationToken token)
    {
        return await _context.WarehouseItems.OrderByDescending(x=>x.AddDate).ToListAsync(token);
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

    public async Task AddItemAsync(GetWarehouseItemRequest item, CancellationToken token)
    {
        var supplier = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == item.SuppliersId, token);
        if (supplier == null)
        {
            throw new InvalidOperationException(string.Format(GlobalMessages.SupplierNotFound, item.SuppliersId));
        }

        var newItem = _mapper.Map<WarehouseItem>(item);
        _context.Entry(newItem).State = EntityState.Modified;
        _context.WarehouseItems.Add(newItem);
        await _context.SaveChangesAsync(token);
    }

    public async Task UpdateItemAsync(GetWarehouseItemRequest item, WarehouseItem oldItem, CancellationToken token)
    {
        var updItem = _mapper.Map<GetWarehouseItemRequest,WarehouseItem>(item);
        updItem.UpdDate = DateTime.Now;
        _context.Entry(oldItem).State = EntityState.Detached; // Detach the old entity
        _context.Entry(updItem).State = EntityState.Modified; // Attach and mark the new entity as modified
        _context.WarehouseItems.Update(updItem);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteItemAsync(int id, CancellationToken token)
    {
       var item = await this.GetSingleItemAsync(id, token);
        if (item == null)
        {
            throw new InvalidOperationException($"WarehouseItem with id {id} not found.");
        }
        _context.WarehouseItems.Remove(item);
        await _context.SaveChangesAsync(token);
    }

    public async Task DeleteMultiItemsAsync(string ids, CancellationToken token)
    {
        var idList = ids.Split(',').Select(int.Parse).ToList();
        var items = await _context.WarehouseItems.Where(x => idList.Contains(x.Id)).ToListAsync(token);
        if (items.Count == 0)
        {
            throw new InvalidOperationException("No items found for deletion.");
        }
        _context.WarehouseItems.RemoveRange(items);
        await _context.SaveChangesAsync(token);
    }

    public async Task<List<Suppliers>> GetSuppliersAsync(CancellationToken token)
    {
        return await _context.Suppliers.ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsBySearchAsync(string searchParam, CancellationToken token)
    {
        var query = _context.WarehouseItems.AsQueryable();
        int searchId = 0;
        int.TryParse(searchParam, out searchId);
        query = query.Where(item =>
            item.Name.Contains(searchParam) ||
            item.UniqueCode.Contains(searchParam) ||
            item.Quantity.ToString().Contains(searchParam) ||
            item.Price.ToString().Contains(searchParam) ||
            item.Suppliers.Name.Contains(searchParam) ||
            item.Id == searchId); 

        return await query.OrderByDescending(x => x.AddDate).ToListAsync(token);
    }

    public async Task<List<WarehouseItem>> GetItemsByFiltersAsync(string? searchParam, string? productName, int? status, int? supplierId, string? startAdd, string? endAdd, string? startUpd, string? endUpd, CancellationToken token)
    {
        var query = _context.WarehouseItems.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(x => x.SuppliersId == supplierId.Value);
        }

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(x => x.Name.Contains(productName.Trim()));
        }

        if (!string.IsNullOrEmpty(startAdd) && DateTime.TryParse(startAdd, out var startAddDate))
        {
            query = query.Where(x => x.AddDate >= startAddDate);
        }

        if (!string.IsNullOrEmpty(endAdd) && DateTime.TryParse(endAdd, out var endAddDate))
        {
            query = query.Where(x => x.AddDate <= endAddDate);
        }

        if (!string.IsNullOrEmpty(startUpd) && DateTime.TryParse(startUpd, out var startUpdDate))
        {
            query = query.Where(x => x.UpdDate >= startUpdDate);
        }

        if (!string.IsNullOrEmpty(endUpd) && DateTime.TryParse(endUpd, out var endUpdDate))
        {
            query = query.Where(x => x.UpdDate <= endUpdDate);
        }

        if (!string.IsNullOrEmpty(searchParam))
        {
            int searchId = 0;
            int.TryParse(searchParam, out searchId);
            query = query.Where(item =>
                item.Name.Contains(searchParam) ||
                item.UniqueCode.Contains(searchParam) ||
                item.Quantity.ToString().Contains(searchParam) ||
                item.Price.ToString().Contains(searchParam) ||
                item.Suppliers.Name.Contains(searchParam) ||
                item.Id == searchId);
        }

        return await query.OrderByDescending(x => x.AddDate).ToListAsync(token);
    }

}

