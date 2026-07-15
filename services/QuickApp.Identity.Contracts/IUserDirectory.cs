using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Read-only query surface for users. Ports the read operations of the monolith's
/// <c>QuickApp.Core.Services.Account.IUserAccountService</c>, replacing the
/// <c>ApplicationUser</c> entity with <see cref="UserDto"/> so callers never see
/// ASP.NET Core Identity persistence types.
/// </summary>
public interface IUserDirectory
{
    /// <summary>Gets a user by opaque id, or <c>null</c> when not found.</summary>
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Lists users, ordered by user name. Pass <c>pageSize = -1</c> to return all.</summary>
    Task<IReadOnlyList<UserDto>> ListUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Lists the role names assigned to a user.</summary>
    Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}
