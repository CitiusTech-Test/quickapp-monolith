using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Service;

/// <summary>
/// Static, dependency-free port of the monolith's
/// <c>QuickApp.Core.Services.Account.ApplicationPermissions</c> catalog. Kept inside
/// the Identity service because centralized permission registration is an Identity
/// responsibility in the target architecture.
/// </summary>
internal static class PermissionCatalog
{
    private const string UsersGroup = "User Permissions";
    private const string RolesGroup = "Role Permissions";

    public static readonly IReadOnlyList<PermissionDto> All =
    [
        new() { Name = "View Users", Value = "users.view", GroupName = UsersGroup, Description = "Permission to view other users account details" },
        new() { Name = "Manage Users", Value = "users.manage", GroupName = UsersGroup, Description = "Permission to create, delete and modify other users account details" },
        new() { Name = "View Roles", Value = "roles.view", GroupName = RolesGroup, Description = "Permission to view available roles" },
        new() { Name = "Manage Roles", Value = "roles.manage", GroupName = RolesGroup, Description = "Permission to create, delete and modify roles" },
        new() { Name = "Assign Roles", Value = "roles.assign", GroupName = RolesGroup, Description = "Permission to assign roles to users" },
    ];
}
