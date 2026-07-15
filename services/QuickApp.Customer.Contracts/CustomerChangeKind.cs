namespace QuickApp.Customer.Contracts
{
    /// <summary>The kind of change described by a <see cref="CustomerChanged"/> integration event.</summary>
    public enum CustomerChangeKind
    {
        Created,
        Updated,
        Deleted
    }
}
