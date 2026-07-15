namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Result of a mutating identity operation. Replaces the
/// <c>(bool Succeeded, string[] Errors)</c> value tuples used throughout the
/// monolith's account services with a named, contract-friendly type.
/// </summary>
public sealed record OperationResult
{
    public required bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>
    /// Populated on a successful create with the new user's opaque id.
    /// </summary>
    public string? UserId { get; init; }

    public static OperationResult Success(string? userId = null) =>
        new() { Succeeded = true, UserId = userId };

    public static OperationResult Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };
}
