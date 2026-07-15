using System.ComponentModel.DataAnnotations;

namespace QuickApp.Product.Service.Models
{
    /// <summary>
    /// Local copy of the monolith's audit/base fields (from <c>QuickApp.Core/Models/BaseEntity.cs</c>).
    /// Copied rather than referenced so the standalone service has no dependency on QuickApp.Core.
    /// </summary>
    public class BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string? CreatedBy { get; set; }

        [MaxLength(40)]
        public string? UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
