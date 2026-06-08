using ProductInventoryApi.Models;

namespace ProductInventoryApi.Services;

public class InventoryService
{
    private readonly IInventoryRepository _repo;

    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<InventoryRecord>> ListAsync(CancellationToken ct = default)
        => _repo.ListAsync(ct);

    public Task<InventoryRecord?> GetAsync(string id, CancellationToken ct = default)
        => _repo.GetAsync(id, ct);

    public async Task<InventoryRecord> UpsertAsync(string productId, int quantity, CancellationToken ct = default)
    {
        if (quantity < 0) throw new ArgumentException("Quantity must be non-negative", nameof(quantity));
        var rec = new InventoryRecord { ProductId = productId, Quantity = quantity };
        return await _repo.UpsertAsync(rec, ct);
    }
}
