# QuickApp Customer Service (decomposition POC)

Standalone skeleton extracted from the QuickApp monolith as part of the microservices
decomposition POC. Owns the **`Customer`** aggregate (`AppCustomers` table).

## Projects
- **`QuickApp.Customer.Contracts`** — `ICustomerService` interface + POCO DTOs
  (`CustomerDto`, `CreateCustomerRequest`, `UpdateCustomerRequest`, `Gender`). No EF/Identity deps.
- **`QuickApp.Customer.Service`** — the `Customer` domain model + a local `BaseEntity` audit copy
  and `InMemoryCustomerService` (deterministic in-memory stub implementing the contract).
- **`QuickApp.Customer.Tests`** — xUnit tests (CRUD round-trip, get-all, top-active).

Build/test: `dotnet test` from the `services/` folder (`QuickApp.Customer.slnx`).

## Coupling broken vs. the monolith
- Removed `Customer.Orders` (`ICollection<Order>`) navigation — the Customer service no longer
  knows about the Order service's entities.
- `GetTopActiveCustomers` no longer runs an EF join over `Orders`. Activity ranking is supplied by
  the Order service via `SetActivityRanking(IReadOnlyList<int>)`; without it the stub falls back to
  a deterministic name ordering.
- Copied the shared-kernel `BaseEntity`/audit fields locally instead of referencing `QuickApp.Core`.

## Angular micro-frontend mapping
This service backs the **`customers`** feature area of the Angular SPA (customer list, profile
create/edit views). In a micro-frontend split it becomes a **`customers` remote** (or lazy-loaded
feature module) mounted by the shell app, talking to this service through the API gateway/BFF.
Shared auth/token-refresh stays in the shell; the `customers` MFE only owns customer UI + its
typed API client.
