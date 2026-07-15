namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Request to update an existing user's profile and (optionally) role assignments.
/// Identified by the opaque user id; exposes no persistence entity or credential data.
/// A null <see cref="Roles"/> means "leave role assignments unchanged".
/// </summary>
public sealed record UpdateUserRequest
{
    public required string Id { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? JobTitle { get; init; }
    public bool? IsEnabled { get; init; }
    public IReadOnlyList<string>? Roles { get; init; }
}
