# Order service extraction session

Work in repository **CitiusTech-Test/quickapp-monolith** on Linux. Open a PR targeting the existing **`modernization-poc`** branch.

## Objective

Extract the Order domain into a standalone, additive project skeleton. Do not remove or rewire the monolith. Create:

- `QuickApp.Order.Contracts`
- `QuickApp.Order.Service`
- `QuickApp.Order.Service.Tests`

## Monolith context

- The solution targets `net10.0`.
- Order entities are `Order` and `OrderDetail` in `QuickApp.Core/Models/Shop`.
- `Order` contains discount/comments and navigations to `Customer` and `ApplicationUser` as cashier.
- `OrderDetail` contains unit price, quantity and discount and navigates to `Product`.
- `ApplicationUser.Orders`, `Customer.Orders` and `Product.OrderDetails` complete the cross-domain object graph.
- `IOrdersService` and `OrdersService` currently exist but are empty.
- `OrderVM` exposes only order header fields.
- `ApplicationDbContext` and `DatabaseSeeder` create orders, products, customers and users in one graph/transaction.
- Angular has a lazy `orders` route, but the component is mostly a placeholder.

## Target responsibilities and ownership

Order owns order headers, lines, discounts, lifecycle state and immutable purchase-time snapshots. It references external domains only through scalar IDs and local abstractions.

Recommended order data includes:

- `CustomerId` plus approved customer snapshot fields
- `CashierUserId` plus approved display/audit snapshot fields
- per-line `ProductId`, product name snapshot, unit price, quantity and discount

Exact snapshot policy requires manual architecture review, so keep the POC contract explicit and minimal.

## Required contract shape

Create clear contracts for representative operations such as:

- place an order
- get order by ID
- list orders
- `OrderPlaced` integration event

Define local service ports for external validation/lookup, such as customer existence and product price/availability. These ports belong to Order's implementation boundary and must not reference another service implementation.

## Implementation requirements

- Follow repository conventions and `ai-rules/AI_RULES.md` where applicable.
- `QuickApp.Order.Contracts` must be a plain class library with DTO/record and interface definitions.
- `QuickApp.Order.Service` must reference Contracts and contain a compilable deterministic stub implementation.
- Add a minimal service registration/composition entry point appropriate for a standalone service skeleton.
- Add `QuickApp.Order.Service.Tests` with passing unit tests for the stub, line snapshots and representative validation.
- Add all projects to `QuickApp.sln`.
- Use nullable reference types and async APIs with cancellation tokens.
- Keep the change focused; no production saga, database migration, Angular rewrite, containers or deployment manifests.

## Coupling rules

- Do not reference `QuickApp.Core`, `QuickApp.Server`, Customer, Product, Identity or Notification implementation projects.
- Do not use `Customer`, `Product` or `ApplicationUser` ORM/navigation types.
- Do not add a distributed transaction or shared database assumption.
- Do not call Notification directly. `OrderPlaced` is the future integration seam.
- Do not decide inventory reservation semantics beyond an explicit abstraction and documented stub behavior.
- Do not modify existing monolith Order behavior.

## Validation and PR

- Run applicable format, build and unit-test commands. If the .NET 10 SDK is absent, install it non-destructively if practical; otherwise document the exact validation limitation in the PR.
- Review the merge-base diff for unrelated changes.
- Commit and push a dedicated branch.
- Open a non-draft PR into `modernization-poc`.
- PR summary must identify `Order` and `OrderDetail` as owned and flag Customer/Product/Identity lookups, snapshot policy, inventory consistency, event delivery and distributed transaction avoidance.

Done when the three projects, interfaces/contracts, stub implementation, passing unit tests where tooling permits, and PR are present.
