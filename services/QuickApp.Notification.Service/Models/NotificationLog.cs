namespace QuickApp.Notification.Service.Models
{
    // Domain model owned by the Notification service. Audit fields (Id/CreatedBy/CreatedDate)
    // are copied locally rather than referencing QuickApp.Core.BaseEntity to keep the service
    // standalone and free of the monolith's EF/Identity dependencies.
    public class NotificationLog
    {
        public int Id { get; set; }

        public string RecipientName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; }

        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }

        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
