using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// In-memory stub implementation of <see cref="IUserRoleService"/>. Behaviour
/// mirrors the monolith's <c>UserRoleService</c> (including permission/claim
/// validation against the permission catalog) but is backed by
/// <see cref="InMemoryIdentityStore"/> dictionaries instead of EF Core + RoleManager.
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly InMemoryIdentityStore _store;
    private readonly IPermissionCatalog _permissions;

    public UserRoleService(InMemoryIdentityStore store, IPermissionCatalog permissions)
    {
        _store = store;
        _permissions = permissions;
    }

    public Task<RoleDto?> GetRoleByIdAsync(string roleId)
    {
        _store.Roles.TryGetValue(roleId, out var role);
        return Task.FromResult(role?.ToDto());
    }

    public Task<RoleDto?> GetRoleByNameAsync(string roleName)
    {
        return Task.FromResult(FindRole(roleName)?.ToDto());
    }

    public Task<RoleDto?> GetRoleLoadRelatedAsync(string roleName)
    {
        // In-memory model already carries permissions + user ids; no eager loading needed.
        return Task.FromResult(FindRole(roleName)?.ToDto());
    }

    public Task<List<RoleDto>> GetRolesLoadRelatedAsync(int page, int pageSize)
    {
        IEnumerable<ApplicationRole> query = _store.Roles.Values.OrderBy(r => r.Name);

        if (page != -1)
            query = query.Skip((page - 1) * pageSize);

        if (pageSize != -1)
            query = query.Take(pageSize);

        return Task.FromResult(query.Select(r => r.ToDto()).ToList());
    }

    public Task<OperationResult> CreateRoleAsync(RoleDto role, IEnumerable<string> claims)
    {
        var claimList = claims.ToArray();

        var invalid = claimList.Where(c => _permissions.GetPermissionByValue(c) == null).ToArray();
        if (invalid.Length != 0)
            return Task.FromResult(OperationResult.Failure(
                $"The following claim types are invalid: {string.Join(", ", invalid)}"));

        if (string.IsNullOrWhiteSpace(role.Name))
            return Task.FromResult(OperationResult.Failure("Role name is required."));

        if (FindRole(role.Name) != null)
            return Task.FromResult(OperationResult.Failure($"Role '{role.Name}' already exists."));

        var entity = new ApplicationRole(role.Name!)
        {
            Id = string.IsNullOrWhiteSpace(role.Id) ? Guid.NewGuid().ToString() : role.Id,
            Description = role.Description,
            CreatedBy = role.CreatedBy,
            UpdatedBy = role.UpdatedBy,
            CreatedDate = role.CreatedDate,
            UpdatedDate = role.UpdatedDate,
        };

        foreach (var claim in claimList.Distinct())
            entity.Permissions.Add(_permissions.GetPermissionByValue(claim)!.Value);

        _store.Roles[entity.Id] = entity;
        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> UpdateRoleAsync(RoleDto role, IEnumerable<string>? claims)
    {
        var claimList = claims?.ToArray();

        if (claimList != null)
        {
            var invalid = claimList.Where(c => _permissions.GetPermissionByValue(c) == null).ToArray();
            if (invalid.Length != 0)
                return Task.FromResult(OperationResult.Failure(
                    $"The following claim types are invalid: {string.Join(", ", invalid)}"));
        }

        if (!_store.Roles.TryGetValue(role.Id, out var entity))
            return Task.FromResult(OperationResult.Failure($"Role '{role.Id}' was not found."));

        entity.Name = role.Name;
        entity.Description = role.Description;
        entity.UpdatedBy = role.UpdatedBy;
        entity.UpdatedDate = role.UpdatedDate;

        if (claimList != null)
        {
            var desired = new HashSet<string>(claimList.Distinct(), StringComparer.Ordinal);
            entity.Permissions.RemoveWhere(p => !desired.Contains(p));
            foreach (var claim in desired)
                entity.Permissions.Add(claim);
        }

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> TestCanDeleteRoleAsync(string roleId)
    {
        var errors = new List<string>();

        if (_store.Roles.TryGetValue(roleId, out var role) && role.UserIds.Count != 0)
            errors.Add("Role has associated users");

        return Task.FromResult(errors.Count == 0
            ? OperationResult.Success()
            : OperationResult.Failure(errors.ToArray()));
    }

    public Task<OperationResult> DeleteRoleAsync(string roleName)
    {
        return FindRole(roleName) is { } role
            ? DeleteRoleAsync(role.ToDto())
            : Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> DeleteRoleAsync(RoleDto role)
    {
        if (_store.Roles.TryRemove(role.Id, out var removed))
        {
            foreach (var user in _store.Users.Values)
            {
                if (removed.Name != null)
                    user.RoleNames.Remove(removed.Name);
            }
        }

        return Task.FromResult(OperationResult.Success());
    }

    private ApplicationRole? FindRole(string? roleName) =>
        _store.Roles.Values.FirstOrDefault(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
}
