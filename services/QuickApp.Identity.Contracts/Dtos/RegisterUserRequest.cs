namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Request to register a new user. Carries only the fields a caller may supply;
/// no persistence entity, password hash or security stamp is exposed. The plain
/// <see cref="Password"/> is accepted at the boundary only — the concrete
/// credential mechanism is an implementation detail of the Identity service.
/// </summary>
public sealed record RegisterUserRequest
{
    public required string UserName { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? JobTitle { get; init; }
    public string? Password { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
}
