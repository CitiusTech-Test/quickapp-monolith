using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// In-memory stub implementation of <see cref="IRoleDirectory"/>. Serves the static
/// <see cref="PermissionCatalog"/> and the roles held in <see cref="InMemoryIdentityStore"/>.
/// </summary>
public sealed class RoleDirectory(InMemoryIdentityStore store) : IRoleDirectory
{
    public Task<IReadOnlyList<RoleDto>> ListRolesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<IdentityRole> query = store.Roles.Values.OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase);

        if (page > 0 && pageSize > 0)
            query = query.Skip((page - 1) * pageSize);

        if (pageSize > 0)
            query = query.Take(pageSize);

        IReadOnlyList<RoleDto> result = query.Select(r => r.ToDto()).ToArray();
        return Task.FromResult(result);
    }

    public Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = store.Roles.Values.FirstOrDefault(r =>
            string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(role?.ToDto());
    }

    public Task<IReadOnlyList<PermissionDto>> ListPermissionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(PermissionCatalog.All);
    }
}
