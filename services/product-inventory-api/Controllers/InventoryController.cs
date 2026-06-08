using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductInventoryApi.Models;
using ProductInventoryApi.Services;

namespace ProductInventoryApi.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _service;

    public InventoryController(InventoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> List()
    {
        if (!HasScope("inventory:read")) return Forbid();
        var list = await _service.ListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(string id)
    {
        if (!HasScope("inventory:read")) return Forbid();
        var rec = await _service.GetAsync(id);
        if (rec is null) return NotFound();
        return Ok(rec);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upsert([FromBody] InventoryRecord input)
    {
        if (!HasScope("inventory:write")) return Forbid();
        if (input.Quantity < 0) return BadRequest(new { error = "Quantity must be non-negative" });
        var updated = await _service.UpsertAsync(input.ProductId, input.Quantity);
        return Ok(updated);
    }

    private bool HasScope(string required)
    {
        var scopeClaim = User.FindFirst(c => c.Type == "scope" || c.Type == "scp");
        if (scopeClaim == null) return false;
        var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return scopes.Contains(required);
    }
}
