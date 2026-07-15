namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain data-transfer representation of an identity user.
/// Deliberately free of any ASP.NET Core Identity or EF Core dependency so the
/// contract can be shared with clients that only know the opaque user id.
/// Never exposes password hashes, security stamps or refresh tokens.
/// </summary>
public sealed record UserDto
{
    /// <summary>
    /// Stable, opaque user identifier. A string to preserve compatibility with
    /// ASP.NET Core Identity; consumers must treat it as an opaque scalar value.
    /// </summary>
    public required string Id { get; init; }

    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? JobTitle { get; init; }
    public bool IsEnabled { get; init; }
    public bool IsLockedOut { get; init; }
}
