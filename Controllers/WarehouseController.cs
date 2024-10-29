using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductsManager.Controllers;
using ProductsManager.Models;
using ProductsManager.Repository;


[Route("warehouse")]
public class WarehouseController : Controller
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

    [HttpGet("main")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        WarehouseViewModel model = new();
        var result = await _repo.GetItemsAsync(token);
        model.Items = result;
        model.Suppliers = await _repo.GetSuppliersAsync(token);
        model.Statuses = Enum.GetValues<Statuses>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });
        
        return View(model);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([Bind("Name,Quantity,Price,SuppliersId,UniqueCode,Status")] GetWarehouseItemRequest item, CancellationToken token)
    {
        try
        {
            await _repo.AddItemAsync(item, token);
            TempData["Success"] = GlobalMessages.ItemCreationSuccess;

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,ex.Message);
            TempData["Error"] = GlobalMessages.ItemCreationError;
            return RedirectToAction("Index");
        }
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit([FromBody] GetWarehouseItemRequest reqItem, CancellationToken token)
    {
        try
        {
            var oldItem = await _repo.GetSingleItemAsync(reqItem.Id, token);
            if (oldItem == null)
            {
                TempData["Error"] = GlobalMessages.ItemNotFound;
                return RedirectToAction("Index");
            }
            await _repo.UpdateItemAsync(reqItem, oldItem, token);
            TempData["Success"] = GlobalMessages.ItemUpdateSuccess;

            return Ok(new { message = GlobalMessages.ItemUpdateSuccess });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Ok(new { Message = GlobalMessages.ItemUpdateError });
        }
    }

    [HttpGet("get-item")]
    public async Task<IActionResult> GetItem(int id, CancellationToken token)
    {
        try
        {
            WarehouseViewModel model = new();
            model.Item = await _repo.GetSingleItemAsync(id, token);

            return Ok(model.Item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Ok(new { Message = GlobalMessages.ItemCancellationError });
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([Bind("cbitems")] string cbitems, CancellationToken token)
    {
        if (string.IsNullOrEmpty(cbitems))
        {
            return NotFound();
        }
        await _repo.DeleteMultiItemsAsync(cbitems, token);
        TempData["Success"] = GlobalMessages.ItemDeletionSuccess;

        return RedirectToAction("Index");
    }

    [HttpGet("search")]
    public async Task<List<WarehouseItem>> SearchWarehouseItems(string searchParam, CancellationToken token)
    {
        if (string.IsNullOrEmpty(searchParam))
        {
            return await _context.WarehouseItems.ToListAsync(token);
        }

        var query = _context.WarehouseItems.AsQueryable();

        query = query.Where(item =>
            item.Name.Contains(searchParam) ||
            item.UniqueCode.Contains(searchParam) ||
            item.Quantity.ToString().Contains(searchParam) ||
            item.Price.ToString().Contains(searchParam) ||
            item.Suppliers.Name.Contains(searchParam));

        return await query.ToListAsync(token);
    }
}
