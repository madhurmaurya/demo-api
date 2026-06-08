using ProductInventoryApi.Models;

namespace ProductInventoryApi.Services;

public interface IInventoryRepository
{
    Task<IEnumerable<InventoryRecord>> ListAsync(CancellationToken ct = default);
    Task<InventoryRecord?> GetAsync(string productId, CancellationToken ct = default);
    Task<InventoryRecord> UpsertAsync(InventoryRecord record, CancellationToken ct = default);
}
