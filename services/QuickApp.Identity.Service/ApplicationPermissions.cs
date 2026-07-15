using System.Collections.ObjectModel;
using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Service;

/// <summary>
/// Permission catalog ported from
/// <c>QuickApp.Core.Services.Account.ApplicationPermissions</c>. The permissions
/// are exposed as plain <see cref="PermissionDto"/> constants (no implicit string
/// conversion operator) and via the dependency-free <see cref="IPermissionCatalog"/>.
/// </summary>
public sealed class ApplicationPermissions : IPermissionCatalog
{
    /************* USER PERMISSIONS *************/

    public const string UsersPermissionGroupName = "User Permissions";

    public static readonly PermissionDto ViewUsers = new()
    {
        Name = "View Users",
        Value = "users.view",
        GroupName = UsersPermissionGroupName,
        Description = "Permission to view other users account details",
    };

    public static readonly PermissionDto ManageUsers = new()
    {
        Name = "Manage Users",
        Value = "users.manage",
        GroupName = UsersPermissionGroupName,
        Description = "Permission to create, delete and modify other users account details",
    };

    /************* ROLE PERMISSIONS *************/

    public const string RolesPermissionGroupName = "Role Permissions";

    public static readonly PermissionDto ViewRoles = new()
    {
        Name = "View Roles",
        Value = "roles.view",
        GroupName = RolesPermissionGroupName,
        Description = "Permission to view available roles",
    };

    public static readonly PermissionDto ManageRoles = new()
    {
        Name = "Manage Roles",
        Value = "roles.manage",
        GroupName = RolesPermissionGroupName,
        Description = "Permission to create, delete and modify roles",
    };

    public static readonly PermissionDto AssignRoles = new()
    {
        Name = "Assign Roles",
        Value = "roles.assign",
        GroupName = RolesPermissionGroupName,
        Description = "Permission to assign roles to users",
    };

    /************* ALL PERMISSIONS *************/

    public static readonly ReadOnlyCollection<PermissionDto> AllPermissions =
        new List<PermissionDto>
        {
            ViewUsers, ManageUsers,
            ViewRoles, ManageRoles, AssignRoles,
        }.AsReadOnly();

    /************* STATIC HELPER METHODS (parity with the monolith) *************/

    public static PermissionDto? GetPermissionByNameStatic(string? permissionName)
    {
        return AllPermissions.SingleOrDefault(p => p.Name == permissionName);
    }

    public static PermissionDto? GetPermissionByValueStatic(string? permissionValue)
    {
        return AllPermissions.SingleOrDefault(p => p.Value == permissionValue);
    }

    public static string[] GetAllPermissionValuesStatic()
    {
        return AllPermissions.Select(p => p.Value).ToArray();
    }

    public static string[] GetAdministrativePermissionValuesStatic()
    {
        return [ManageUsers.Value, ManageRoles.Value, AssignRoles.Value];
    }

    /************* IPermissionCatalog *************/

    public IReadOnlyList<PermissionDto> GetAllPermissions() => AllPermissions;

    public string[] GetAllPermissionValues() => GetAllPermissionValuesStatic();

    public string[] GetAdministrativePermissionValues() => GetAdministrativePermissionValuesStatic();

    public PermissionDto? GetPermissionByName(string? permissionName) => GetPermissionByNameStatic(permissionName);

    public PermissionDto? GetPermissionByValue(string? permissionValue) => GetPermissionByValueStatic(permissionValue);
}
