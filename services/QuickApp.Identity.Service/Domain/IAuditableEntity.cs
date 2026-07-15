namespace QuickApp.Identity.Service.Domain;

/// <summary>
/// Local copy of the monolith's shared-kernel audit contract
/// (<c>QuickApp.Core.Models.IAuditableEntity</c>). Duplicated per service so the
/// Identity service does not take a runtime dependency on QuickApp.Core.
/// </summary>
public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    DateTime UpdatedDate { get; set; }
}
