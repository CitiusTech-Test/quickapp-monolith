namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain data-transfer representation of an identity role, including the
/// permission values granted to it. No ASP.NET Core Identity dependency.
/// </summary>
public sealed record RoleDto
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }

    /// <summary>Permission values (see <see cref="PermissionDto.Value"/>) granted to this role.</summary>
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
