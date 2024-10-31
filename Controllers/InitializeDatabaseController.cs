using Microsoft.AspNetCore.Mvc;
using ProductsManager.Services;

namespace ProductsManager.Controllers;

[ApiController]
[Route("system")]
public class InitializeDatabaseController : ControllerBase
{
    private ILogger<InitializeDatabaseController> _logger;
    private ISystemService _service;

    public InitializeDatabaseController(ISystemService service, ILogger<InitializeDatabaseController> logger)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("db-init")]
    public async Task<ActionResult<string>> InitializeDatabaseAsync(CancellationToken token)
    {
        try
        {
            await _service.InitializeDatabaseAsync(token);

            return RedirectToAction("Index", "Warehouse");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialize database");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}

