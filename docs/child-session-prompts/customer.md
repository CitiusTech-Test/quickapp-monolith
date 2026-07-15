# Customer service extraction session

Work in repository **CitiusTech-Test/quickapp-monolith** on Linux. Open a PR targeting the existing **`modernization-poc`** branch.

## Objective

Extract the Customer domain into a standalone, additive project skeleton. Do not remove or rewire the monolith. Create:

- `QuickApp.Customer.Contracts`
- `QuickApp.Customer.Service`
- `QuickApp.Customer.Service.Tests`

## Monolith context

- The solution targets `net10.0`.
- `Customer` is in `QuickApp.Core/Models/Shop/Customer.cs` with name, email, phone, address, city and gender.
- `Customer.Orders` crosses the proposed Customer-to-Order boundary.
- `CustomerService.GetAllCustomersData` eagerly loads Orders, OrderDetails, Product and Cashier, making Customer a distributed-join risk.
- `CustomerController` exposes `GET api/customer` and directly uses both `ICustomerService` and `IEmailSender`.
- `CustomerVM` includes an Orders collection.
- Customer mapping is in the shared `MappingProfile`.
- Angular has a lazy `customers` route, but its component is mostly a placeholder.
- `ApplicationDbContext` and `DatabaseSeeder` currently persist and seed Customer with every other domain.

## Target responsibilities and ownership

Customer owns customer profile, contact and address data. It does not own order history, product data, user accounts or message delivery.

The future Customer API returns customer-owned data only. Order history should be provided by Order or a gateway/BFF read model.

## Required contract shape

Create clear contracts for representative operations such as:

- get customer by ID
- list/search customers
- create/update a customer
- a `CustomerChanged` integration event

Preserve the current integer customer ID for the POC. Contract DTOs must not contain `Order`, `OrderDetail`, `Product`, `ApplicationUser` or monolith ViewModel types.

## Implementation requirements

- Follow repository conventions and `ai-rules/AI_RULES.md` where applicable.
- `QuickApp.Customer.Contracts` must be a plain class library with DTO/record and interface definitions.
- `QuickApp.Customer.Service` must reference Contracts and contain a compilable deterministic stub implementation.
- Add a minimal service registration/composition entry point appropriate for a standalone service skeleton.
- Add `QuickApp.Customer.Service.Tests` with passing unit tests for the stub and contract behavior.
- Add all projects to `QuickApp.sln`.
- Use nullable reference types and async APIs with cancellation tokens.
- Keep the change focused; no production database migration, Angular rewrite, containers or deployment manifests.

## Coupling rules

- Do not reference `QuickApp.Core`, `QuickApp.Server`, Order, Product, Identity or Notification implementation projects.
- Do not include an Orders navigation or order-history collection in Customer contracts.
- Do not reproduce `GetAllCustomersData` as a cross-domain object graph.
- Do not inject an email sender into Customer. Publish/return a customer event or result for later Notification integration.
- Do not modify existing monolith Customer behavior.

## Validation and PR

- Run applicable format, build and unit-test commands. If the .NET 10 SDK is absent, install it non-destructively if practical; otherwise document the exact validation limitation in the PR.
- Review the merge-base diff for unrelated changes.
- Commit and push a dedicated branch.
- Open a non-draft PR into `modernization-poc`.
- PR summary must identify `Customer` as owned and flag the current `Customer.Orders`, eager-loaded object graph, shared DbContext, shared seeding and direct email dependency.

Done when the three projects, interfaces/contracts, stub implementation, passing unit tests where tooling permits, and PR are present.
