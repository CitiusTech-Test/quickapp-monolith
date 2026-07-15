using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Read-only access to the application permission catalog. Ports the lookup
/// helpers from <c>QuickApp.Core.Services.Account.ApplicationPermissions</c>.
/// </summary>
public interface IPermissionCatalog
{
    IReadOnlyList<PermissionDto> GetAllPermissions();

    string[] GetAllPermissionValues();

    string[] GetAdministrativePermissionValues();

    PermissionDto? GetPermissionByName(string? permissionName);

    PermissionDto? GetPermissionByValue(string? permissionValue);
}
