using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Service.Tests;

public class UserDirectoryTests
{
    private static (UserDirectory directory, UserRegistrationService registration) CreateSut()
    {
        var store = new InMemoryIdentityStore();
        var publisher = new RecordingIntegrationEventPublisher();
        return (new UserDirectory(store), new UserRegistrationService(store, publisher));
    }

    [Fact]
    public async Task GetUserByIdAsync_returns_registered_user()
    {
        var (directory, registration) = CreateSut();
        var created = await registration.RegisterUserAsync(new RegisterUserRequest
        {
            UserName = "frank",
            Email = "frank@example.com",
        });

        var user = await directory.GetUserByIdAsync(created.UserId!);

        Assert.NotNull(user);
        Assert.Equal("frank", user!.UserName);
        Assert.Equal("frank@example.com", user.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_returns_null_for_unknown_id()
    {
        var (directory, _) = CreateSut();
        Assert.Null(await directory.GetUserByIdAsync("nope"));
    }

    [Fact]
    public async Task ListUsersAsync_returns_all_ordered_by_username()
    {
        var (directory, registration) = CreateSut();
        await registration.RegisterUserAsync(new RegisterUserRequest { UserName = "zoe" });
        await registration.RegisterUserAsync(new RegisterUserRequest { UserName = "amy" });

        var users = await directory.ListUsersAsync(page: -1, pageSize: -1);

        Assert.Equal(["amy", "zoe"], users.Select(u => u.UserName));
    }

    [Fact]
    public async Task ListUsersAsync_paginates()
    {
        var (directory, registration) = CreateSut();
        await registration.RegisterUserAsync(new RegisterUserRequest { UserName = "a" });
        await registration.RegisterUserAsync(new RegisterUserRequest { UserName = "b" });
        await registration.RegisterUserAsync(new RegisterUserRequest { UserName = "c" });

        var page = await directory.ListUsersAsync(page: 2, pageSize: 1);

        var only = Assert.Single(page);
        Assert.Equal("b", only.UserName);
    }

    [Fact]
    public async Task GetUserRolesAsync_returns_assigned_roles_sorted()
    {
        var (directory, registration) = CreateSut();
        var created = await registration.RegisterUserAsync(new RegisterUserRequest
        {
            UserName = "grace",
            Roles = ["user", "administrator"],
        });

        var roles = await directory.GetUserRolesAsync(created.UserId!);

        Assert.Equal(["administrator", "user"], roles);
    }
}
