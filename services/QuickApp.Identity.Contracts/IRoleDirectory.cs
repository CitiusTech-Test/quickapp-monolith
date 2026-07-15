using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Read-only query surface for roles and the permission catalog. Ports the read
/// operations of the monolith's <c>IUserRoleService</c> and
/// <c>ApplicationPermissions</c>, returning dependency-free DTOs.
/// </summary>
public interface IRoleDirectory
{
    Task<IReadOnlyList<RoleDto>> ListRolesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>Lists the full application permission catalog.</summary>
    Task<IReadOnlyList<PermissionDto>> ListPermissionsAsync(CancellationToken cancellationToken = default);
}
