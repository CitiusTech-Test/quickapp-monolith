# QuickApp.Identity.Service

Standalone, additive skeleton for the extracted **Identity** domain. This does **not**
replace or rewire the monolith — it is a target-shape reference for the decomposition POC.

## Projects

- **QuickApp.Identity.Contracts** — plain class library: DTOs/records, interfaces and the
  `UserRegistered` integration event. No ASP.NET Core Identity, EF Core or Shop types.
- **QuickApp.Identity.Service** — compilable stub implementation backed by an in-memory
  store, with a DI composition entry point (`AddIdentityServiceStub`).
- **QuickApp.Identity.Service.Tests** — xUnit tests for the stub and contract behavior.

## Ownership

Identity owns users, credentials, roles, claims, permissions, OpenIddict clients/tokens
and token issuance. It publishes stable, **opaque** string user identifiers and claims.
It does **not** own orders, customers, products or notifications.

## Integration seam

`RegisterUserAsync` publishes a `UserRegistered` event via `IIntegrationEventPublisher`.
Downstream services (e.g. Notification) consume this event; Identity takes no direct
dependency on them. The skeleton ships with a `NoOpIntegrationEventPublisher` default.

## Coupling points still owned by the monolith

Broken/deferred boundaries called out by the decomposition:

- **`Order.Cashier` / `ApplicationUser.Orders`** — the DB-level Identity↔Order dependency.
  Not copied here; the stub's "can delete user" logic no longer reads `Orders`.
- **Shared `ApplicationDbContext`** — Identity/OpenIddict tables still share a context with
  Customer/Product/Order in the monolith.
- **Audit user claims** — `IAuditableEntity.CreatedBy`/`UpdatedBy` embed user ids across domains.
- **Centralized permission registration** — the permission catalog is authored by Identity
  but consumed by every service's authorization layer.

## Build & test

```bash
dotnet build services/QuickApp.Identity.Contracts/QuickApp.Identity.Contracts.csproj
dotnet build services/QuickApp.Identity.Service/QuickApp.Identity.Service.csproj
dotnet test  services/QuickApp.Identity.Service.Tests/QuickApp.Identity.Service.Tests.csproj
```

Requires the **.NET 10** SDK (`net10.0`).
