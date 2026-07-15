namespace QuickApp.Identity.Contracts.Dtos;

/// <summary>
/// Plain data-transfer representation of an identity role, including its
/// permission claim values. No ASP.NET Core Identity dependency.
/// </summary>
public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// Permission values (see <c>PermissionDto.Value</c>) granted to this role.
    /// </summary>
    public IList<string> Permissions { get; set; } = new List<string>();

    /// <summary>
    /// Ids of the users assigned to this role.
    /// </summary>
    public IList<string> UserIds { get; set; } = new List<string>();

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
