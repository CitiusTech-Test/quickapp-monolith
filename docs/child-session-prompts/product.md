# Product service extraction session

Work in repository **CitiusTech-Test/quickapp-monolith** on Linux. Open a PR targeting the existing **`modernization-poc`** branch.

## Objective

Extract the Product domain into a standalone, additive project skeleton. Do not remove or rewire the monolith. Create:

- `QuickApp.Product.Contracts`
- `QuickApp.Product.Service`
- `QuickApp.Product.Service.Tests`

## Monolith context

- The solution targets `net10.0`.
- Product entities are `Product` and `ProductCategory` in `QuickApp.Core/Models/Shop`.
- Product owns name, description, icon, buying/selling price, stock, active/discontinued state and category.
- `Product.Parent`/`Children` is a self-reference inside the Product boundary and may remain represented by scalar IDs.
- `Product.OrderDetails` and `OrderDetail.Product` cross the proposed Product-to-Order boundary.
- `IProductService` and `ProductService` currently exist but are empty.
- `ProductVM` and shared AutoMapper mappings exist.
- Angular has a lazy `products` route, but the component is mostly a placeholder.
- `ApplicationDbContext` and `DatabaseSeeder` currently persist and seed Product with Customer and Order.

## Target responsibilities and ownership

Product owns catalog, categories, commercial price and stock state. It provides product lookup and availability/price information to Order but does not own order lines or order history.

Order must snapshot the product name and unit price used at purchase time rather than rely on a Product database join.

## Required contract shape

Create clear contracts for representative operations such as:

- get product by ID
- list/search products
- get categories
- obtain current price/availability
- a `ProductChanged` or `StockChanged` integration event

Preserve integer product/category IDs for the POC. Contracts must not expose `OrderDetail` or monolith entities/ViewModels.

## Implementation requirements

- Follow repository conventions and `ai-rules/AI_RULES.md` where applicable.
- `QuickApp.Product.Contracts` must be a plain class library with DTO/record and interface definitions.
- `QuickApp.Product.Service` must reference Contracts and contain a compilable deterministic stub implementation.
- Add a minimal service registration/composition entry point appropriate for a standalone service skeleton.
- Add `QuickApp.Product.Service.Tests` with passing unit tests for the stub and important contract behavior.
- Add all projects to `QuickApp.sln`.
- Use nullable reference types and async APIs with cancellation tokens.
- Keep the change focused; no production database migration, Angular rewrite, containers or deployment manifests.

## Coupling rules

- Do not reference `QuickApp.Core`, `QuickApp.Server`, Order, Customer, Identity or Notification implementation projects.
- Do not include `OrderDetails` or order-history queries in Product contracts.
- Model product hierarchy with optional parent IDs, not shared ORM navigation.
- Do not make Product call Order.
- Do not modify existing monolith Product behavior.

## Validation and PR

- Run applicable format, build and unit-test commands. If the .NET 10 SDK is absent, install it non-destructively if practical; otherwise document the exact validation limitation in the PR.
- Review the merge-base diff for unrelated changes.
- Commit and push a dedicated branch.
- Open a non-draft PR into `modernization-poc`.
- PR summary must identify `Product` and `ProductCategory` as owned and flag `OrderDetail.Product`, price/inventory consistency, shared DbContext and shared seeding.

Done when the three projects, interfaces/contracts, stub implementation, passing unit tests where tooling permits, and PR are present.
