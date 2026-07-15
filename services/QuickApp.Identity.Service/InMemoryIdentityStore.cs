using System.Collections.Concurrent;
using QuickApp.Identity.Service.Domain;

namespace QuickApp.Identity.Service;

/// <summary>
/// Thread-safe in-memory backing store for the stub services. Replaces the single
/// <c>ApplicationDbContext</c> plus ASP.NET Identity managers used by the monolith.
/// POC-only state with no persistence; passwords are held in plain text purely so
/// the stub is self-contained — this is not a credential mechanism.
/// </summary>
public sealed class InMemoryIdentityStore
{
    internal ConcurrentDictionary<string, IdentityUser> Users { get; } = new(StringComparer.Ordinal);

    internal ConcurrentDictionary<string, IdentityRole> Roles { get; } = new(StringComparer.Ordinal);

    internal ConcurrentDictionary<string, string> Passwords { get; } = new(StringComparer.Ordinal);
}
