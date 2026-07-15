using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Role management contract for the Identity service.
/// Method signatures are ported from
/// <c>QuickApp.Core.Services.Account.IUserRoleService</c>, with the
/// <c>ApplicationRole</c> parameter/return types replaced by <see cref="RoleDto"/>
/// and the <c>(bool, string[])</c> tuples replaced by <see cref="OperationResult"/>.
/// </summary>
public interface IUserRoleService
{
    Task<OperationResult> CreateRoleAsync(RoleDto role, IEnumerable<string> claims);

    Task<OperationResult> DeleteRoleAsync(RoleDto role);

    Task<OperationResult> DeleteRoleAsync(string roleName);

    Task<RoleDto?> GetRoleByIdAsync(string roleId);

    Task<RoleDto?> GetRoleByNameAsync(string roleName);

    Task<RoleDto?> GetRoleLoadRelatedAsync(string roleName);

    Task<List<RoleDto>> GetRolesLoadRelatedAsync(int page, int pageSize);

    Task<OperationResult> TestCanDeleteRoleAsync(string roleId);

    Task<OperationResult> UpdateRoleAsync(RoleDto role, IEnumerable<string>? claims);
}
