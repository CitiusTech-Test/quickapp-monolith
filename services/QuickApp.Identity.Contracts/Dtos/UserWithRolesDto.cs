namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// A user together with the names of the roles they belong to. Replaces the
/// <c>(ApplicationUser User, string[] Roles)</c> tuple used in the monolith.
/// </summary>
public class UserWithRolesDto
{
    public UserDto User { get; set; } = new();
    public string[] Roles { get; set; } = [];
}
