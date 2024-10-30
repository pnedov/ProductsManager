using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductsManager.Controllers;
using ProductsManager.Models;
using ProductsManager.Repository;
using System.Collections.Generic;


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
    public async Task<IActionResult> SearchWarehouseItems(string searchParam, CancellationToken token)
    {
        WarehouseViewModel model = new();
        if (string.IsNullOrEmpty(searchParam))
        {
            model.Items = await _repo.GetItemsAsync(token);
        }
        else
        {
            model.Items = await _repo.GetItemsBySearchAsync(searchParam, token);
        }
        model.Suppliers = await _repo.GetSuppliersAsync(token);
        model.Statuses = Enum.GetValues<Statuses>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });
        model.SearchString = searchParam;

        return View("Index", model);
    }

    [HttpGet("filters")]
    public async Task<IActionResult> FilterWarehouseItems(string? searchParam, string? productName, int? status, int? suppliersid, string? start, string? end, string? startUpd, string? endUpd, CancellationToken token)
    {
        WarehouseViewModel model = new();
        if (!status.HasValue && !suppliersid.HasValue && string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
        {
            model.Items = await _repo.GetItemsAsync(token);
        }
        else
        {
            model.Items = await _repo.GetItemsByFiltersAsync(searchParam, productName, status, suppliersid, start, end, startUpd, endUpd, token);
        }
        model.Suppliers = await _repo.GetSuppliersAsync(token);
        model.Statuses = Enum.GetValues<Statuses>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

        model.Filters = new WarehouseItemFilters()
        {
            Status = status,
            SuppliersId = suppliersid,
            StartAddSate = start,
            EndAddDate = end,
            ProductName = productName
        };

        return View("Index", model);
    }
}
