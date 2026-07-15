# QuickApp Order service skeleton

This standalone POC owns `Order` and `OrderDetail` and exposes `IOrdersService`.
It uses an in-memory store plus pluggable `ICustomerLookup` and `IProductLookup`
contracts so callers can replace the included lookup stubs with service clients.

## Coupling removed

- The cashier is stored as opaque `CashierUserId` text from the JWT `sub` claim.
- Customer identity is stored as `CustomerId` with name and email snapshots.
- Product identity is stored as `ProductId` with name and unit-price snapshots.
- No EF Core, ASP.NET Core Identity, `ApplicationDbContext`, or `QuickApp.Core`
  references are used.

The Angular `orders` feature maps to this service. In a later phase it can become
an orders micro-frontend behind the shared shell/BFF while continuing to consume
customer and product choices through service APIs.
