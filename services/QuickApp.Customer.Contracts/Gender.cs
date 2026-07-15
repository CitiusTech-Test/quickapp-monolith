namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Customer gender. The Customer service owns its own copy of this value type rather than
    /// depending on the monolith's shared <c>QuickApp.Core.Models.Gender</c>.
    /// </summary>
    public enum Gender
    {
        None,
        Female,
        Male
    }
}
