namespace QuickApp.Order.Service.Models;

/// <summary>
/// Order-owned base entity. Intentionally a local copy so the service does not
/// depend on QuickApp.Core; only the audit fields the domain needs are kept.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
