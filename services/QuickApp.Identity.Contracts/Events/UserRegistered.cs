namespace QuickApp.Identity.Contracts.Events;

/// <summary>
/// Integration event raised when a new user is registered in the Identity service.
/// This is the future integration seam for downstream services (e.g. Notification)
/// which must react to user creation without taking a direct dependency on Identity.
/// Carries only opaque, stable identifiers and non-sensitive profile data.
/// </summary>
public sealed record UserRegistered
{
    /// <summary>Opaque, stable identifier of the newly registered user.</summary>
    public required string UserId { get; init; }

    public required string UserName { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }

    /// <summary>Role names the user was assigned at registration time.</summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
