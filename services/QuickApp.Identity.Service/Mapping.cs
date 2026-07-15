using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

internal static class Mapping
{
    public static UserDto ToDto(this IdentityUser user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        FullName = user.FullName,
        JobTitle = user.JobTitle,
        IsEnabled = user.IsEnabled,
        IsLockedOut = user.IsLockedOut,
    };

    public static RoleDto ToDto(this IdentityRole role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        Permissions = role.Permissions.ToArray(),
    };
}
