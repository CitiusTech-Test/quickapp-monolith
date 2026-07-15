namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Result of a mutating identity operation. Replaces the
/// <c>(bool Succeeded, string[] Errors)</c> value tuples used throughout the
/// monolith's account services with a named, contract-friendly type.
/// </summary>
public readonly record struct OperationResult(bool Succeeded, string[] Errors)
{
    public static OperationResult Success() => new(true, []);

    public static OperationResult Failure(params string[] errors) => new(false, errors);
}
