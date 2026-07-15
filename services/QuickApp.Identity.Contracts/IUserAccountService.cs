using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// User account management contract for the Identity service.
/// Method signatures are ported from
/// <c>QuickApp.Core.Services.Account.IUserAccountService</c>, with the
/// <c>ApplicationUser</c> parameter/return types replaced by <see cref="UserDto"/>
/// and the <c>(bool, string[])</c> tuples replaced by <see cref="OperationResult"/>.
/// </summary>
public interface IUserAccountService
{
    Task<bool> CheckPasswordAsync(string userId, string password);

    Task<OperationResult> CreateUserAsync(UserDto user, IEnumerable<string> roles, string password);

    Task<OperationResult> DeleteUserAsync(UserDto user);

    Task<OperationResult> DeleteUserAsync(string userId);

    Task<UserWithRolesDto?> GetUserAndRolesAsync(string userId);

    Task<UserDto?> GetUserByEmailAsync(string email);

    Task<UserDto?> GetUserByIdAsync(string userId);

    Task<UserDto?> GetUserByUserNameAsync(string userName);

    Task<IList<string>> GetUserRolesAsync(string userId);

    Task<List<UserWithRolesDto>> GetUsersAndRolesAsync(int page, int pageSize);

    Task<OperationResult> ResetPasswordAsync(UserDto user, string newPassword);

    Task<OperationResult> TestCanDeleteUserAsync(string userId);

    Task<OperationResult> UpdatePasswordAsync(UserDto user, string currentPassword, string newPassword);

    Task<OperationResult> UpdateUserAsync(UserDto user);

    Task<OperationResult> UpdateUserAsync(UserDto user, IEnumerable<string>? roles);
}
