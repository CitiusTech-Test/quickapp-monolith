using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Service.Tests;

public class UserRegistrationServiceTests
{
    private static (UserRegistrationService service, InMemoryIdentityStore store, RecordingIntegrationEventPublisher publisher) CreateSut()
    {
        var store = new InMemoryIdentityStore();
        var publisher = new RecordingIntegrationEventPublisher();
        return (new UserRegistrationService(store, publisher), store, publisher);
    }

    [Fact]
    public async Task RegisterUserAsync_creates_user_and_returns_id()
    {
        var (service, _, _) = CreateSut();

        var result = await service.RegisterUserAsync(new RegisterUserRequest
        {
            UserName = "alice",
            Email = "alice@example.com",
            Roles = ["administrator"],
        });

        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(result.UserId));
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task RegisterUserAsync_publishes_UserRegistered_event()
    {
        var (service, _, publisher) = CreateSut();

        var result = await service.RegisterUserAsync(new RegisterUserRequest
        {
            UserName = "bob",
            Email = "bob@example.com",
            FullName = "Bob Builder",
            Roles = ["user", "user"],
        });

        var published = Assert.Single(publisher.Published);
        Assert.Equal(result.UserId, published.UserId);
        Assert.Equal("bob", published.UserName);
        Assert.Equal("bob@example.com", published.Email);
        Assert.Equal(["user"], published.Roles);
    }

    [Fact]
    public async Task RegisterUserAsync_rejects_blank_username()
    {
        var (service, _, publisher) = CreateSut();

        var result = await service.RegisterUserAsync(new RegisterUserRequest { UserName = "  " });

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
        Assert.Empty(publisher.Published);
    }

    [Fact]
    public async Task RegisterUserAsync_rejects_duplicate_username_case_insensitively()
    {
        var (service, _, publisher) = CreateSut();

        await service.RegisterUserAsync(new RegisterUserRequest { UserName = "carol" });
        var result = await service.RegisterUserAsync(new RegisterUserRequest { UserName = "CAROL" });

        Assert.False(result.Succeeded);
        Assert.Single(publisher.Published);
    }

    [Fact]
    public async Task UpdateUserAsync_updates_profile_and_roles()
    {
        var (service, _, _) = CreateSut();
        var created = await service.RegisterUserAsync(new RegisterUserRequest { UserName = "dave", Roles = ["user"] });

        var result = await service.UpdateUserAsync(new UpdateUserRequest
        {
            Id = created.UserId!,
            FullName = "Dave Updated",
            Roles = ["administrator"],
        });

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateUserAsync_fails_for_unknown_user()
    {
        var (service, _, _) = CreateSut();

        var result = await service.UpdateUserAsync(new UpdateUserRequest { Id = "does-not-exist" });

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task RegisterUserAsync_honors_cancellation()
    {
        var (service, _, _) = CreateSut();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            service.RegisterUserAsync(new RegisterUserRequest { UserName = "erin" }, cts.Token));
    }
}
