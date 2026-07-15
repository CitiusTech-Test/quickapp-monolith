using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service.Tests;

public class RoleDirectoryTests
{
    private static (RoleDirectory directory, InMemoryIdentityStore store) CreateSut()
    {
        var store = new InMemoryIdentityStore();
        return (new RoleDirectory(store), store);
    }

    private static void SeedRole(InMemoryIdentityStore store, string id, string name, params string[] permissions)
    {
        var role = new IdentityRole { Id = id, Name = name };
        role.Permissions.AddRange(permissions);
        store.Roles[id] = role;
    }

    [Fact]
    public async Task ListPermissionsAsync_returns_full_catalog()
    {
        var (directory, _) = CreateSut();

        var permissions = await directory.ListPermissionsAsync();

        Assert.Equal(5, permissions.Count);
        Assert.Contains(permissions, p => p.Value == "users.manage");
        Assert.Contains(permissions, p => p.Value == "roles.assign");
        Assert.All(permissions, p => Assert.False(string.IsNullOrWhiteSpace(p.GroupName)));
    }

    [Fact]
    public async Task ListRolesAsync_returns_seeded_roles_ordered_by_name()
    {
        var (directory, store) = CreateSut();
        SeedRole(store, "2", "user");
        SeedRole(store, "1", "administrator", "users.manage");

        var roles = await directory.ListRolesAsync(page: -1, pageSize: -1);

        Assert.Equal(["administrator", "user"], roles.Select(r => r.Name));
        Assert.Equal(["users.manage"], roles.First().Permissions);
    }

    [Fact]
    public async Task GetRoleByNameAsync_is_case_insensitive()
    {
        var (directory, store) = CreateSut();
        SeedRole(store, "1", "Administrator");

        var role = await directory.GetRoleByNameAsync("administrator");

        Assert.NotNull(role);
        Assert.Equal("Administrator", role!.Name);
    }

    [Fact]
    public async Task GetRoleByNameAsync_returns_null_when_missing()
    {
        var (directory, _) = CreateSut();
        Assert.Null(await directory.GetRoleByNameAsync("ghost"));
    }
}
