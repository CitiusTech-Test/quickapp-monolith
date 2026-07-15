namespace QuickApp.Identity.Service.Domain;

/// <summary>
/// Internal, framework-free user record for the stub store. Deliberately NOT the
/// monolith's <c>ApplicationUser</c>: it carries no <c>Orders</c> navigation and no
/// ASP.NET Core Identity base type, so Identity owns nothing from the Shop domain.
/// </summary>
internal sealed class IdentityUser
{
    public required string Id { get; init; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? JobTitle { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsLockedOut { get; set; }
    public HashSet<string> Roles { get; } = new(StringComparer.OrdinalIgnoreCase);
}
