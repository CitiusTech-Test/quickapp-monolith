# QuickApp Order service skeleton

Additive proof-of-concept that extracts the **Order** domain from the monolith
into a standalone project. It does **not** remove or rewire the monolith.

## Ownership

The service owns order **headers**, **lines**, **discounts**, **lifecycle state**
(`OrderStatus`) and **immutable purchase-time snapshots**. `Order` and
`OrderDetail` (local `Models`) are the owned entities.

External domains are referenced only through:

- scalar ids (`CustomerId`, `CashierUserId`, `ProductId`), and
- approved snapshot fields (customer name/email, product name, unit price).

No `QuickApp.Core`, `QuickApp.Server`, EF Core, ASP.NET Core Identity,
`ApplicationDbContext`, or `Customer`/`Product`/`ApplicationUser` ORM types are
referenced.

## Contracts and ports (`QuickApp.Order.Contracts`)

- `IOrdersService` — place an order, get by id, list orders.
- `ICustomerLookup` — validate customer existence / resolve snapshot.
- `IProductLookup` — resolve product price and availability.
- `IInventoryReservation` — explicit stock-reservation abstraction.
- `IOrderEventPublisher` + `OrderPlaced` — the future integration seam.

## Stubs (deterministic, in-memory)

- `InMemoryOrdersService` — validates, snapshots, reserves, publishes.
- `InMemoryCustomerLookup` / `InMemoryProductLookup` — fixed snapshot sets.
- `NoOpInventoryReservation` — **no real reservation; always succeeds.**
- `InMemoryOrderEventPublisher` — **records events in memory only.**

Register everything through `services.AddOrderService()`. A host supplies real
`ICustomerLookup` / `IProductLookup` adapters.

## Deliberately out of scope (needs architecture review)

- Exact customer/cashier snapshot policy.
- Inventory reservation semantics (holds, timeouts, oversell, compensation).
- `OrderPlaced` delivery guarantees (bus/broker/outbox).
- Any distributed transaction or shared-database assumption — avoided by design;
  Notification is reached only via the `OrderPlaced` event, never called directly.
