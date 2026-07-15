namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain representation of an application permission from the permission catalog.
/// Mirrors <c>QuickApp.Core.Models.Account.ApplicationPermission</c> without the
/// implicit string-conversion operator, keeping the contract dependency-free.
/// </summary>
public sealed record PermissionDto
{
    public required string Name { get; init; }
    public required string Value { get; init; }
    public required string GroupName { get; init; }
    public string? Description { get; init; }
}
