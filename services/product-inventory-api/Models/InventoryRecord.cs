using System.Text.Json.Serialization;

namespace ProductInventoryApi.Models;

public record InventoryRecord
{
    public string ProductId { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

    [JsonIgnore]
    public Product? Product { get; init; }
}
