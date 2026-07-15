using Xunit;
using QuickApp.Identity.Contracts;
using QuickApp.Identity.Service;

namespace QuickApp.Identity.Tests;

public class PermissionCatalogTests
{
    private static IPermissionCatalog CreateSut() => new ApplicationPermissions();

    [Fact]
    public void GetAllPermissions_returns_full_catalog()
    {
        var catalog = CreateSut();

        var all = catalog.GetAllPermissions();

        Assert.Equal(5, all.Count);
        Assert.Equal(
            new[] { "users.view", "users.manage", "roles.view", "roles.manage", "roles.assign" },
            catalog.GetAllPermissionValues());
    }

    [Fact]
    public void LookupByValue_and_byName_resolve_permission()
    {
        var catalog = CreateSut();

        var byValue = catalog.GetPermissionByValue("users.manage");
        var byName = catalog.GetPermissionByName("Manage Users");

        Assert.NotNull(byValue);
        Assert.Equal("Manage Users", byValue!.Name);
        Assert.NotNull(byName);
        Assert.Equal("users.manage", byName!.Value);
    }

    [Fact]
    public void Lookup_unknown_permission_returns_null()
    {
        var catalog = CreateSut();

        Assert.Null(catalog.GetPermissionByValue("nope"));
        Assert.Null(catalog.GetPermissionByName("Nope"));
    }

    [Fact]
    public void AdministrativePermissions_are_the_manage_and_assign_values()
    {
        var catalog = CreateSut();

        var admin = catalog.GetAdministrativePermissionValues();

        Assert.Equal(new[] { "users.manage", "roles.manage", "roles.assign" }, admin);
    }
}
