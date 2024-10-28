using FluentValidation;
using FluentValidation.Results;
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
    private readonly IValidator<GetWarehouseItemRequest> _validator;

    public WarehouseController(IValidator<GetWarehouseItemRequest> validator, WarehouseDbContext context, IWarehouseItemRepository repo, ILogger<InitializeDatabaseController> logger)
    {
        _context = context;
        _repo = repo;
        _logger = logger;
        _validator = validator;  
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
        ValidationResult result = await _validator.ValidateAsync(item, token);

        try
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                //return BadRequest(ModelState);
                return RedirectToAction("Index");
            }

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

    [HttpGet("delete")]
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
}
