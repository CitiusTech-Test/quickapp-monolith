namespace QuickApp.Identity.Service.Domain;

/// <summary>
/// Internal, framework-free role record for the stub store.
/// </summary>
internal sealed class IdentityRole
{
    public required string Id { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string> Permissions { get; } = [];
}
