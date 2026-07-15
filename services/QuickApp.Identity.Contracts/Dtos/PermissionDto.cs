namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain representation of an application permission from the permission catalog.
/// Mirrors <c>QuickApp.Core.Models.Account.ApplicationPermission</c> without the
/// implicit string conversion operator, keeping the contract dependency-free.
/// </summary>
public class PermissionDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
