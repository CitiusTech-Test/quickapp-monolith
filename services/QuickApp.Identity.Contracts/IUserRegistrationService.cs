using QuickApp.Identity.Contracts.Dtos;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Write surface for creating and updating users without exposing persistence
/// entities. A successful <see cref="RegisterUserAsync"/> publishes a
/// <see cref="Events.UserRegistered"/> integration event.
/// </summary>
public interface IUserRegistrationService
{
    Task<OperationResult> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
}
