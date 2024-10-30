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
    public async Task<IActionResult> FilterWarehouseItems(string searchParam, string? productName, int? status, int? suppliersid, string? start_add, string? end_add, string? start_upd, string? end_upd, CancellationToken token)
    {
        WarehouseViewModel model = new();
        var result = await _repo.GetItemsByFiltersAsync(searchParam, productName, status, suppliersid, start_add, end_add, start_upd, end_upd, token);
        model = await PopulateItemsAsync(result, token);
        model.Filters = new WarehouseItemFilters()
        {
            Status = status,
            SuppliersId = suppliersid,
            StartAddSate = start_add,
            EndAddDate = end_add,
            EndUpdDate = end_upd,
            StartUpdDate = start_upd,
            ProductName = productName,
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
        var allItems = await _repo.GetItemsAsync(token);
        model.Products = allItems.Select(item => item.Name).Distinct().ToList();
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
