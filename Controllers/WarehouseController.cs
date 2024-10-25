using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsManager.Controllers;
using ProductsManager.Models;
using ProductsManager.Repository;

public class WarehouseController : Controller // Inherit from Controller
{
    private ILogger<InitializeDatabaseController> _logger;
    private readonly WarehouseDbContext _context;
    private readonly IWarehouseItemRepository _repo;

    public WarehouseController(WarehouseDbContext context, IWarehouseItemRepository repo, ILogger<InitializeDatabaseController> logger)
    {
        _context = context;
        _repo = repo;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken token)
    {
        var result = await _repo.GetItemsAsync(token);

        WarehouseViewModel viewModel = new();
        viewModel.Items = result;

        return View(viewModel); // View method is now available

    }

    // GET: Warehouse/Create
    public IActionResult Create()
    {
        return Ok();
    }

    // POST: Warehouse/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,Supplier,UniqueCode,Status,SuppliersId")] WarehouseItem item, CancellationToken token)
    {
        if (ModelState.IsValid)
        {
            await _repo.AddItemAsync(item, token);

            return RedirectToAction(nameof(Index));
        }

        return Ok(item);
    }

    // GET: Warehouse/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id, [FromBody] WarehouseItem item, CancellationToken token)
    {
        var oldItem = await _repo.GetSingleItemAsync(item.Id, token);
        if (oldItem == null)
        {
            return Ok(item);
        }

        //oldItem.Name = item.Name;
        //oldItem.Quantity = item.Quantity;
        //oldItem.Price = item.Price;
        //oldItem.UniqueCode = item.UniqueCode;

        await _repo.UpdateItemAsync(item, token);

        return Ok(item);
    }

    // POST: Warehouse/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(int id, [Bind("Id,Name,Quantity,Price,Supplier,UniqueCode,Status,SuppliersId")] WarehouseItem item, CancellationToken token)
    {
        if (id != item.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _repo.UpdateItemAsync(item, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating item");

                return RedirectToAction(nameof(Index));
            }
        }

        return Ok(item);
    }

    // GET: Warehouse/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var warehouseItem = await _context.WarehouseItems
            .FirstOrDefaultAsync(m => m.Id == id);
        if (warehouseItem == null)
        {
            return NotFound();
        }

        return Ok(warehouseItem);
    }

    // POST: Warehouse/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        _context.WarehouseItems.Remove(item);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool WarehouseItemExists(int id)
    {
        return _context.WarehouseItems.Any(e => e.Id == id);
    }
}
