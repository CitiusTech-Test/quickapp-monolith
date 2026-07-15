using Xunit;
using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service;

namespace QuickApp.Identity.Tests;

public class UserAccountServiceTests
{
    private static (IUserAccountService accounts, IUserRoleService roles, InMemoryIdentityStore store) CreateSut()
    {
        var store = new InMemoryIdentityStore();
        var catalog = new ApplicationPermissions();
        return (new UserAccountService(store), new UserRoleService(store, catalog), store);
    }

    private static UserDto NewUser(string userName = "alice", string? id = null) => new()
    {
        Id = id ?? Guid.NewGuid().ToString(),
        UserName = userName,
        Email = $"{userName}@example.com",
        FullName = "Alice Example",
        IsEnabled = true,
    };

    [Fact]
    public async Task CreateUser_persists_user_and_password()
    {
        var (accounts, _, _) = CreateSut();
        var user = NewUser();

        var result = await accounts.CreateUserAsync(user, ["admin"], "P@ssw0rd!");

        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);

        var fetched = await accounts.GetUserByIdAsync(user.Id);
        Assert.NotNull(fetched);
        Assert.Equal("alice", fetched!.UserName);
        Assert.True(await accounts.CheckPasswordAsync(user.Id, "P@ssw0rd!"));
    }

    [Fact]
    public async Task CreateUser_rejects_duplicate_username()
    {
        var (accounts, _, _) = CreateSut();
        await accounts.CreateUserAsync(NewUser(), [], "pw");

        var result = await accounts.CreateUserAsync(NewUser(id: Guid.NewGuid().ToString()), [], "pw");

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task AssignRole_via_update_reflects_in_user_roles()
    {
        var (accounts, roles, _) = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "admin" }, [ApplicationPermissions.ManageUsers.Value]);
        var user = NewUser();
        await accounts.CreateUserAsync(user, [], "pw");

        var result = await accounts.UpdateUserAsync(user, ["admin"]);

        Assert.True(result.Succeeded);
        var userRoles = await accounts.GetUserRolesAsync(user.Id);
        Assert.Contains("admin", userRoles);

        var role = await roles.GetRoleByNameAsync("admin");
        Assert.Contains(user.Id, role!.UserIds);
    }

    [Fact]
    public async Task ResetPassword_changes_password()
    {
        var (accounts, _, _) = CreateSut();
        var user = NewUser();
        await accounts.CreateUserAsync(user, [], "old");

        var result = await accounts.ResetPasswordAsync(user, "new");

        Assert.True(result.Succeeded);
        Assert.False(await accounts.CheckPasswordAsync(user.Id, "old"));
        Assert.True(await accounts.CheckPasswordAsync(user.Id, "new"));
    }

    [Fact]
    public async Task UpdatePassword_requires_correct_current_password()
    {
        var (accounts, _, _) = CreateSut();
        var user = NewUser();
        await accounts.CreateUserAsync(user, [], "old");

        var wrong = await accounts.UpdatePasswordAsync(user, "bad", "new");
        Assert.False(wrong.Succeeded);

        var ok = await accounts.UpdatePasswordAsync(user, "old", "new");
        Assert.True(ok.Succeeded);
        Assert.True(await accounts.CheckPasswordAsync(user.Id, "new"));
    }

    [Fact]
    public async Task DeleteUser_removes_user_and_role_membership()
    {
        var (accounts, roles, _) = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "admin" }, []);
        var user = NewUser();
        await accounts.CreateUserAsync(user, ["admin"], "pw");

        var result = await accounts.DeleteUserAsync(user.Id);

        Assert.True(result.Succeeded);
        Assert.Null(await accounts.GetUserByIdAsync(user.Id));
        var role = await roles.GetRoleByNameAsync("admin");
        Assert.DoesNotContain(user.Id, role!.UserIds);
    }

    [Fact]
    public async Task GetUsersAndRoles_supports_paging_and_returns_roles()
    {
        var (accounts, roles, _) = CreateSut();
        await roles.CreateRoleAsync(new RoleDto { Name = "admin" }, []);
        await accounts.CreateUserAsync(NewUser("alice"), ["admin"], "pw");
        await accounts.CreateUserAsync(NewUser("bob"), [], "pw");

        var all = await accounts.GetUsersAndRolesAsync(-1, -1);
        Assert.Equal(2, all.Count);

        var alice = all.Single(u => u.User.UserName == "alice");
        Assert.Contains("admin", alice.Roles);

        var firstPage = await accounts.GetUsersAndRolesAsync(1, 1);
        Assert.Single(firstPage);
    }

    [Fact]
    public async Task TestCanDeleteUser_succeeds_without_cross_service_order_check()
    {
        var (accounts, _, _) = CreateSut();
        var user = NewUser();
        await accounts.CreateUserAsync(user, [], "pw");

        var result = await accounts.TestCanDeleteUserAsync(user.Id);

        Assert.True(result.Succeeded);
    }
}
