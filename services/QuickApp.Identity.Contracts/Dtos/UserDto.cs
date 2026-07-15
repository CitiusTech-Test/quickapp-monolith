namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain data-transfer representation of an identity user.
/// Deliberately free of any ASP.NET Core Identity or EF Core dependency so the
/// contract can be shared with resource-server clients that only know the user id.
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? FullName { get; set; }
    public string? Configuration { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsLockedOut { get; set; }
    public string? PhoneNumber { get; set; }

    // Audit fields (snapshot of the shared kernel BaseEntity/IAuditableEntity contract).
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
