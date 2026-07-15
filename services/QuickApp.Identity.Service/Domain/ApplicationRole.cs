namespace QuickApp.Identity.Service.Domain;

/// <summary>
/// Domain model for a role owned by the Identity service. Plain POCO port of
/// <c>QuickApp.Core.Models.Account.ApplicationRole</c> without the ASP.NET Core
/// Identity base class. Permission claims are held as plain permission values.
/// </summary>
public class ApplicationRole : IAuditableEntity
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
    {
        Name = roleName;
    }

    public ApplicationRole(string roleName, string description)
    {
        Name = roleName;
        Description = description;
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }

    /// <summary>Permission values granted to this role (the monolith stores these as claims).</summary>
    public HashSet<string> Permissions { get; } = new(StringComparer.Ordinal);

    /// <summary>Ids of the users assigned to this role (in-service reference, no FK).</summary>
    public HashSet<string> UserIds { get; } = new(StringComparer.Ordinal);

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
