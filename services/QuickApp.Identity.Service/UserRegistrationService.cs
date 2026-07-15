using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Contracts.Events;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// In-memory stub implementation of <see cref="IUserRegistrationService"/>.
/// A successful registration publishes a <see cref="UserRegistered"/> integration
/// event through <see cref="IIntegrationEventPublisher"/> — the future seam for
/// downstream services such as Notification, which is intentionally NOT a direct
/// dependency here.
/// </summary>
public sealed class UserRegistrationService(
    InMemoryIdentityStore store,
    IIntegrationEventPublisher eventPublisher) : IUserRegistrationService
{
    public async Task<OperationResult> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.UserName))
            return OperationResult.Failure("User name is required.");

        if (store.Users.Values.Any(u => string.Equals(u.UserName, request.UserName, StringComparison.OrdinalIgnoreCase)))
            return OperationResult.Failure($"User name '{request.UserName}' is already taken.");

        var roles = request.Roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            JobTitle = request.JobTitle,
        };

        foreach (var role in roles)
            user.Roles.Add(role);

        store.Users[user.Id] = user;
        if (request.Password is not null)
            store.Passwords[user.Id] = request.Password;

        await eventPublisher.PublishAsync(
            new UserRegistered
            {
                UserId = user.Id,
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                Roles = roles,
            },
            cancellationToken);

        return OperationResult.Success(user.Id);
    }

    public Task<OperationResult> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!store.Users.TryGetValue(request.Id, out var user))
            return Task.FromResult(OperationResult.Failure($"User '{request.Id}' was not found."));

        if (request.Email is not null)
            user.Email = request.Email;
        if (request.FullName is not null)
            user.FullName = request.FullName;
        if (request.JobTitle is not null)
            user.JobTitle = request.JobTitle;
        if (request.IsEnabled is { } enabled)
            user.IsEnabled = enabled;

        if (request.Roles is not null)
        {
            user.Roles.Clear();
            foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
                user.Roles.Add(role);
        }

        return Task.FromResult(OperationResult.Success(user.Id));
    }
}
