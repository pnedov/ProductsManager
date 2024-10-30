using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductsManager.Controllers;
using ProductsManager.Models;
using ProductsManager.Repository;


[Route("report")]
public class ReportController : Controller
{
    private ILogger<InitializeDatabaseController> _logger;
    private readonly WarehouseDbContext _context;
    private readonly IWarehouseItemRepository _repo;

    public ReportController(WarehouseDbContext context, IWarehouseItemRepository repo, ILogger<InitializeDatabaseController> logger)
    {
        _context = context;
        _repo = repo;
        _logger = logger;  
    }

    [HttpGet("main")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        WarehouseViewModel model = new();
        List<WarehouseItem> result = await _repo.GetItemsAsync(token);
        model = await PopulateItemsAsync(result, token);

        return View(model);
    }

    [HttpGet("filters")]
    public async Task<IActionResult> FilterWarehouseItems(string searchParam, int? status, int? suppliersid, string? start, string? end, CancellationToken token)
    {
        WarehouseViewModel model = new();
        var result = await _repo.GetItemsByFiltersAsync(searchParam, status, suppliersid, start, end, token);
        model = await PopulateItemsAsync(result, token);
        model.Filters = new WarehouseItemFilters()
        {
            Status = status,
            SuppliersId = suppliersid,
            Start = start,
            End = end,
            SearchString = searchParam
        };
       
        return View("Index", model);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchWarehouseItems(string searchParam, CancellationToken token)
    {
        WarehouseViewModel model = new();
        if (string.IsNullOrEmpty(searchParam))
        {
            var result = await _repo.GetItemsAsync(token);
            model = await PopulateItemsAsync(result, token);
        }
        else
        {
            var result = await _repo.GetItemsBySearchAsync(searchParam, token);
            model = await PopulateItemsAsync(result, token);
        }

        model.SearchString = searchParam;

        return View("Index", model);
    }

    private async Task<WarehouseViewModel> PopulateItemsAsync(List<WarehouseItem> items, CancellationToken token)
    {
        WarehouseViewModel model = new();
        model.Items = items;
        model.TotalRecords = items.Count;
        model.TotalProducts = items.Select(item => item.Name).Distinct().Count();
        model.TotalPrice = items.Sum(item => item.Price);
        model.TotalQuantity = items.Sum(item => item.Quantity);
        model.TotalSuppliers = items.Select(item => item.SuppliersId).Distinct().Count();
        model.Suppliers = await _repo.GetSuppliersAsync(token);
        model.Statuses = Enum.GetValues<Statuses>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

        return model;
    }
}
