namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Criteria for listing/searching customers. A null or empty <see cref="Query"/> lists all
    /// customers. Paging is expressed with <see cref="Skip"/> and <see cref="Take"/>.
    /// </summary>
    public sealed record CustomerSearchRequest
    {
        /// <summary>Case-insensitive term matched against name, email and city. Null lists all.</summary>
        public string? Query { get; init; }

        public int Skip { get; init; }

        public int Take { get; init; } = 50;
    }
}
