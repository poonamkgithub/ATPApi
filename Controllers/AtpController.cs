using ATPApi.Data;
using ATPApi.DTOs;
using ATPApi.Models;
using ATPApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATPApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtpController : ControllerBase
{
    private readonly IAtpService _service;
    private readonly AtpDbContext _db;

    public AtpController(IAtpService service, AtpDbContext db)
    {
        _service = service;
        _db = db;
    }

    /// <summary>Calculate available-to-promise quantity for a product and location.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string productCode, [FromQuery] string locationCode,
        [FromQuery] DateTime? requestedDate)
    {
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(locationCode))
            return BadRequest(new { message = "productCode and locationCode are required." });

        var date = requestedDate ?? DateTime.Today;
        return Ok(await _service.GetAtpAsync(productCode, locationCode, date));
    }

    /// <summary>Check whether a requested quantity can be fulfilled.</summary>
    [HttpPost("check")]
    public async Task<IActionResult> Check([FromBody] CheckAtpRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        return Ok(await _service.CheckAvailabilityAsync(request));
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventory() =>
        Ok(await _db.Inventories.AsNoTracking().OrderBy(x => x.ProductCode).ToListAsync());

    [HttpPost("inventory")]
    public async Task<IActionResult> UpsertInventory([FromBody] Inventory input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var existing = await _db.Inventories.FirstOrDefaultAsync(x =>
            x.ProductCode == input.ProductCode && x.LocationCode == input.LocationCode);
        if (existing == null)
        {
            input.UpdatedAtUtc = DateTime.UtcNow;
            _db.Inventories.Add(input);
        }
        else
        {
            existing.OnHandQuantity = input.OnHandQuantity;
            existing.ReservedQuantity = input.ReservedQuantity;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return Ok(existing ?? input);
    }

    [HttpGet("incoming-supply")]
    public async Task<IActionResult> GetIncomingSupply() =>
        Ok(await _db.IncomingSupplies.AsNoTracking().OrderBy(x => x.ExpectedDate).ToListAsync());

    [HttpPost("incoming-supply")]
    public async Task<IActionResult> AddIncomingSupply([FromBody] IncomingSupply input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        _db.IncomingSupplies.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetIncomingSupply), new { id = input.Id }, input);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders() =>
        Ok(await _db.CustomerOrders.AsNoTracking().OrderBy(x => x.RequiredDate).ToListAsync());

    [HttpPost("orders")]
    public async Task<IActionResult> AddOrder([FromBody] CustomerOrder input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        _db.CustomerOrders.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOrders), new { id = input.Id }, input);
    }
}
