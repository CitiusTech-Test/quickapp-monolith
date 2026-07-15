namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Copied from the monolith's <c>QuickApp.Core.Models.Gender</c> so the Customer service
    /// owns its own value type rather than depending on the shared kernel.
    /// </summary>
    public enum Gender
    {
        None,
        Female,
        Male
    }
}
