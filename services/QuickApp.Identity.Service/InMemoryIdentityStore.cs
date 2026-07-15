using System.Collections.Concurrent;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// Shared in-memory backing store for the stub user/role services. Dictionary-backed
/// and thread-safe; replaces the single <c>ApplicationDbContext</c> + Identity managers
/// used by the monolith. This is POC-only state (no persistence).
/// </summary>
public sealed class InMemoryIdentityStore
{
    public ConcurrentDictionary<string, ApplicationUser> Users { get; } = new(StringComparer.Ordinal);

    public ConcurrentDictionary<string, ApplicationRole> Roles { get; } = new(StringComparer.Ordinal);

    /// <summary>Plain-text passwords keyed by user id. POC-only — never do this in production.</summary>
    public ConcurrentDictionary<string, string> Passwords { get; } = new(StringComparer.Ordinal);
}
