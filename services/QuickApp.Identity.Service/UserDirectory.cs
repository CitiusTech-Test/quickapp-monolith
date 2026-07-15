using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// In-memory stub implementation of <see cref="IUserDirectory"/>. Deterministic and
/// dependency-free; backed by <see cref="InMemoryIdentityStore"/> rather than EF Core.
/// </summary>
public sealed class UserDirectory(InMemoryIdentityStore store) : IUserDirectory
{
    public Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        store.Users.TryGetValue(userId, out var user);
        return Task.FromResult(user?.ToDto());
    }

    public Task<IReadOnlyList<UserDto>> ListUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<IdentityUser> query = store.Users.Values.OrderBy(u => u.UserName, StringComparer.OrdinalIgnoreCase);

        if (page > 0 && pageSize > 0)
            query = query.Skip((page - 1) * pageSize);

        if (pageSize > 0)
            query = query.Take(pageSize);

        IReadOnlyList<UserDto> result = query.Select(u => u.ToDto()).ToArray();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<string> roles = store.Users.TryGetValue(userId, out var user)
            ? user.Roles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToArray()
            : [];

        return Task.FromResult(roles);
    }
}
