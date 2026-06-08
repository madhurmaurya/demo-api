using ProductInventoryApi.Models;

namespace ProductInventoryApi.Services;

public class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly Dictionary<string, InventoryRecord> _store = new();
    private readonly object _lock = new();

    public Task<IEnumerable<InventoryRecord>> ListAsync(CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_store.Values.AsEnumerable());
        }
    }

    public Task<InventoryRecord?> GetAsync(string productId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _store.TryGetValue(productId, out var rec);
            return Task.FromResult(rec);
        }
    }

    public Task<InventoryRecord> UpsertAsync(InventoryRecord record, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var updated = record with { LastUpdated = DateTime.UtcNow };
            _store[record.ProductId] = updated;
            return Task.FromResult(updated);
        }
    }
}
