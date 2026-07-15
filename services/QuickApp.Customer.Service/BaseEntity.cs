using System.ComponentModel.DataAnnotations;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// Local copy of the monolith's shared-kernel audit contract
    /// (<c>QuickApp.Core.Models.BaseEntity</c> / <c>IAuditableEntity</c>). Duplicated per the
    /// decomposition proposal so the Customer service does not take a runtime dependency on
    /// <c>QuickApp.Core</c>.
    /// </summary>
    public class BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string? CreatedBy { get; set; }

        [MaxLength(40)]
        public string? UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
