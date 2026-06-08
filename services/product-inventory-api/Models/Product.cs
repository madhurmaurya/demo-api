namespace ProductInventoryApi.Models;

public record Product
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
