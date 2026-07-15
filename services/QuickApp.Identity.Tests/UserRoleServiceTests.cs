using Xunit;
using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service;

namespace QuickApp.Identity.Tests;

public class UserRoleServiceTests
{
    private static IUserRoleService CreateSut()
    {
        var store = new InMemoryIdentityStore();
        return new UserRoleService(store, new ApplicationPermissions());
    }

    [Fact]
    public async Task CreateRole_stores_valid_permissions()
    {
        var roles = CreateSut();

        var result = await roles.CreateRoleAsync(
            new RoleDto { Name = "admin", Description = "Administrators" },
            [ApplicationPermissions.ManageUsers.Value, ApplicationPermissions.ManageRoles.Value]);

        Assert.True(result.Succeeded);
        var role = await roles.GetRoleByNameAsync("admin");
        Assert.NotNull(role);
        Assert.Contains(ApplicationPermissions.ManageUsers.Value, role!.Permissions);
        Assert.Contains(ApplicationPermissions.ManageRoles.Value, role.Permissions);
    }

    [Fact]
    public async Task CreateRole_rejects_invalid_permission()
    {
        var roles = CreateSut();

        var result = await roles.CreateRoleAsync(new RoleDto { Name = "bad" }, ["does.not.exist"]);

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task UpdateRole_replaces_permission_set()
    {
        var roles = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "admin" }, [ApplicationPermissions.ViewUsers.Value]);
        var role = await roles.GetRoleByNameAsync("admin");

        var result = await roles.UpdateRoleAsync(role!, [ApplicationPermissions.ManageUsers.Value]);

        Assert.True(result.Succeeded);
        var updated = await roles.GetRoleByNameAsync("admin");
        Assert.DoesNotContain(ApplicationPermissions.ViewUsers.Value, updated!.Permissions);
        Assert.Contains(ApplicationPermissions.ManageUsers.Value, updated.Permissions);
    }

    [Fact]
    public async Task DeleteRole_by_name_removes_role()
    {
        var roles = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "temp" }, []);

        var result = await roles.DeleteRoleAsync("temp");

        Assert.True(result.Succeeded);
        Assert.Null(await roles.GetRoleByNameAsync("temp"));
    }

    [Fact]
    public async Task GetRolesLoadRelated_returns_all_roles()
    {
        var roles = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "admin" }, []);
        await roles.CreateRoleAsync(new RoleDto { Name = "user" }, []);

        var all = await roles.GetRolesLoadRelatedAsync(-1, -1);

        Assert.Equal(2, all.Count);
    }
}
