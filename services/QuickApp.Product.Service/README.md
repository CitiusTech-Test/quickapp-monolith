# QuickApp.Product.Service

Standalone skeleton of the **Product** service, extracted from the QuickApp monolith as part of
the microservices decomposition POC.

## What this service owns
- `Product` (`AppProducts`) — buying/selling price, stock, active/discontinued flags, a
  self-referencing `Parent`/`Children` hierarchy, and a `ProductCategory` relationship.
- `ProductCategory` (`AppProductCategories`).

## Coupling broken during extraction
- The monolith `Product` entity has `ICollection<OrderDetail> OrderDetails`, an EF navigation into
  the **Order** service. This navigation is **removed** from the standalone model. Order-to-product
  links now live on the Order side as a plain `ProductId` (+ price/name snapshot) value.
- `BaseEntity` audit fields are **copied** locally (`Models/BaseEntity.cs`) instead of referencing
  `QuickApp.Core`, so this project has no dependency on the monolith.

## Preserved (owned by this service)
- Self-referencing `ParentId` / `Parent` / `Children`.
- `ProductCategoryId` / `ProductCategory`.

## Contract & stub
- Contract: `QuickApp.Product.Contracts.IProductService` (CRUD + stock adjust + categories).
- Stub: `InMemoryProductService` — deterministic in-memory store for tests/demos. Replace with an
  EF Core / repository-backed implementation for production.

## Angular micro-frontend mapping
This service maps to the Angular **products** feature area
(`quickapp.client/src/app/.../products`). As a micro-frontend it would own the product list,
product editor, and category management views, calling this service's REST API instead of the
monolith's shared controllers.
