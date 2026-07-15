namespace QuickApp.Identity.Service.Domain;

/// <summary>
/// Domain model for a user owned by the Identity service.
/// This is a plain POCO port of <c>QuickApp.Core.Models.Account.ApplicationUser</c>
/// with the ASP.NET Core Identity base class and all cross-service navigation
/// properties (notably <c>Orders</c> → Shop) removed. Roles are tracked here by
/// name only rather than via EF Identity join entities.
/// </summary>
public class ApplicationUser : IAuditableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? JobTitle { get; set; }
    public string? FullName { get; set; }
    public string? Configuration { get; set; }
    public bool IsEnabled { get; set; }

    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool IsLockedOut => LockoutEnabled && LockoutEnd >= DateTimeOffset.UtcNow;

    public string? FriendlyName
    {
        get
        {
            var friendlyName = string.IsNullOrWhiteSpace(FullName) ? UserName : FullName;

            if (!string.IsNullOrWhiteSpace(JobTitle))
                friendlyName = $"{JobTitle} {friendlyName}";

            return friendlyName;
        }
    }

    /// <summary>Names of the roles this user belongs to (in-service reference, no FK).</summary>
    public HashSet<string> RoleNames { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
