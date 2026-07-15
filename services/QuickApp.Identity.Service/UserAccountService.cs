using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// In-memory stub implementation of <see cref="IUserAccountService"/>.
/// Behaviour mirrors the monolith's <c>UserAccountService</c> where practical but
/// is backed by <see cref="InMemoryIdentityStore"/> dictionaries instead of EF Core
/// + ASP.NET Identity. Deterministic and dependency-free for POC unit testing.
///
/// NOTE (coupling broken): the monolith's <c>TestCanDeleteUserAsync</c> queried
/// <c>_context.Orders.Where(o =&gt; o.CashierId == userId)</c> — a direct read of a
/// Shop-owned entity. That cross-service check is removed here; in the target
/// architecture Order is a separate service that references the user only by id
/// (<c>Order.CashierUserId</c>) and would answer this via its own API/event.
/// </summary>
public class UserAccountService : IUserAccountService
{
    private readonly InMemoryIdentityStore _store;

    public UserAccountService(InMemoryIdentityStore store)
    {
        _store = store;
    }

    public Task<UserDto?> GetUserByIdAsync(string userId)
    {
        _store.Users.TryGetValue(userId, out var user);
        return Task.FromResult(user?.ToDto());
    }

    public Task<UserDto?> GetUserByUserNameAsync(string userName)
    {
        var user = _store.Users.Values.FirstOrDefault(u =>
            string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user?.ToDto());
    }

    public Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = _store.Users.Values.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user?.ToDto());
    }

    public Task<IList<string>> GetUserRolesAsync(string userId)
    {
        IList<string> roles = _store.Users.TryGetValue(userId, out var user)
            ? user.RoleNames.OrderBy(r => r).ToList()
            : [];
        return Task.FromResult(roles);
    }

    public Task<UserWithRolesDto?> GetUserAndRolesAsync(string userId)
    {
        if (!_store.Users.TryGetValue(userId, out var user))
            return Task.FromResult<UserWithRolesDto?>(null);

        return Task.FromResult<UserWithRolesDto?>(new UserWithRolesDto
        {
            User = user.ToDto(),
            Roles = user.RoleNames.OrderBy(r => r).ToArray(),
        });
    }

    public Task<List<UserWithRolesDto>> GetUsersAndRolesAsync(int page, int pageSize)
    {
        IEnumerable<ApplicationUser> query = _store.Users.Values.OrderBy(u => u.UserName);

        if (page != -1)
            query = query.Skip((page - 1) * pageSize);

        if (pageSize != -1)
            query = query.Take(pageSize);

        var result = query
            .Select(u => new UserWithRolesDto
            {
                User = u.ToDto(),
                Roles = u.RoleNames.OrderBy(r => r).ToArray(),
            })
            .ToList();

        return Task.FromResult(result);
    }

    public Task<OperationResult> CreateUserAsync(UserDto user, IEnumerable<string> roles, string password)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            return Task.FromResult(OperationResult.Failure("User name is required."));

        if (_store.Users.Values.Any(u => string.Equals(u.UserName, user.UserName, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult(OperationResult.Failure($"User name '{user.UserName}' is already taken."));

        var entity = new ApplicationUser { Id = string.IsNullOrWhiteSpace(user.Id) ? Guid.NewGuid().ToString() : user.Id };
        entity.ApplyFrom(user);

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            entity.RoleNames.Add(role);
            if (_store.Roles.Values.FirstOrDefault(r =>
                string.Equals(r.Name, role, StringComparison.OrdinalIgnoreCase)) is { } roleEntity)
            {
                roleEntity.UserIds.Add(entity.Id);
            }
        }

        _store.Users[entity.Id] = entity;
        _store.Passwords[entity.Id] = password;

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> UpdateUserAsync(UserDto user) => UpdateUserAsync(user, null);

    public Task<OperationResult> UpdateUserAsync(UserDto user, IEnumerable<string>? roles)
    {
        if (!_store.Users.TryGetValue(user.Id, out var entity))
            return Task.FromResult(OperationResult.Failure($"User '{user.Id}' was not found."));

        entity.ApplyFrom(user);

        if (roles != null)
        {
            var desired = new HashSet<string>(roles.Distinct(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

            var toRemove = entity.RoleNames.Where(r => !desired.Contains(r)).ToArray();
            var toAdd = desired.Where(r => !entity.RoleNames.Contains(r)).ToArray();

            foreach (var role in toRemove)
            {
                entity.RoleNames.Remove(role);
                if (FindRole(role) is { } roleEntity)
                    roleEntity.UserIds.Remove(entity.Id);
            }

            foreach (var role in toAdd)
            {
                entity.RoleNames.Add(role);
                if (FindRole(role) is { } roleEntity)
                    roleEntity.UserIds.Add(entity.Id);
            }
        }

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> ResetPasswordAsync(UserDto user, string newPassword)
    {
        if (!_store.Users.ContainsKey(user.Id))
            return Task.FromResult(OperationResult.Failure($"User '{user.Id}' was not found."));

        _store.Passwords[user.Id] = newPassword;
        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> UpdatePasswordAsync(UserDto user, string currentPassword, string newPassword)
    {
        if (!_store.Users.ContainsKey(user.Id))
            return Task.FromResult(OperationResult.Failure($"User '{user.Id}' was not found."));

        if (!_store.Passwords.TryGetValue(user.Id, out var stored) || stored != currentPassword)
            return Task.FromResult(OperationResult.Failure("Incorrect password."));

        _store.Passwords[user.Id] = newPassword;
        return Task.FromResult(OperationResult.Success());
    }

    public Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var ok = _store.Passwords.TryGetValue(userId, out var stored) && stored == password;
        return Task.FromResult(ok);
    }

    public Task<OperationResult> TestCanDeleteUserAsync(string userId)
    {
        // Cross-service coupling removed: the monolith checked for associated Orders
        // via the shared DbContext. Order is now a separate service; it references the
        // user by id only and would enforce this rule itself. Always deletable here.
        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> DeleteUserAsync(string userId)
    {
        return _store.Users.TryGetValue(userId, out var user)
            ? DeleteUserAsync(user.ToDto())
            : Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> DeleteUserAsync(UserDto user)
    {
        if (_store.Users.TryRemove(user.Id, out var removed))
        {
            _store.Passwords.TryRemove(user.Id, out _);
            foreach (var role in removed.RoleNames)
            {
                if (FindRole(role) is { } roleEntity)
                    roleEntity.UserIds.Remove(removed.Id);
            }
        }

        return Task.FromResult(OperationResult.Success());
    }

    private ApplicationRole? FindRole(string roleName) =>
        _store.Roles.Values.FirstOrDefault(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
}
