using QuickApp.Identity.Contracts.Dtos;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// Maps between the internal domain models and the dependency-free contract DTOs.
/// </summary>
internal static class Mapping
{
    public static UserDto ToDto(this ApplicationUser user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        JobTitle = user.JobTitle,
        FullName = user.FullName,
        Configuration = user.Configuration,
        IsEnabled = user.IsEnabled,
        IsLockedOut = user.IsLockedOut,
        CreatedBy = user.CreatedBy,
        UpdatedBy = user.UpdatedBy,
        CreatedDate = user.CreatedDate,
        UpdatedDate = user.UpdatedDate,
    };

    /// <summary>Applies mutable DTO fields onto an existing domain user.</summary>
    public static void ApplyFrom(this ApplicationUser user, UserDto dto)
    {
        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.JobTitle = dto.JobTitle;
        user.FullName = dto.FullName;
        user.Configuration = dto.Configuration;
        user.IsEnabled = dto.IsEnabled;
        user.CreatedBy = dto.CreatedBy;
        user.UpdatedBy = dto.UpdatedBy;
        user.CreatedDate = dto.CreatedDate;
        user.UpdatedDate = dto.UpdatedDate;
    }

    public static RoleDto ToDto(this ApplicationRole role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        Permissions = role.Permissions.ToList(),
        UserIds = role.UserIds.ToList(),
        CreatedBy = role.CreatedBy,
        UpdatedBy = role.UpdatedBy,
        CreatedDate = role.CreatedDate,
        UpdatedDate = role.UpdatedDate,
    };
}
