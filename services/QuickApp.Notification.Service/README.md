# QuickApp.Notification.Service

Standalone skeleton for the **Notification** microservice extracted from the QuickApp monolith
as part of the decomposition POC.

## What it owns
Outbound notifications. Today the monolith's contract is `IEmailSender`
(`QuickApp.Core/Services/IEmailSender.cs`), implemented by `EmailSender`
(`QuickApp.Server/Services/Email/EmailSender.cs`, MailKit SMTP). Notification is a **leaf**
service — nothing else depends on it — so it is a natural first extraction.

## Projects
- **QuickApp.Notification.Contracts** — `IEmailSender` (copied verbatim), the
  `INotificationHandler<T>` subscription contract, and the plain-POCO event DTOs
  `OrderPlaced` / `UserRegistered`.
- **QuickApp.Notification.Service** — `FakeEmailSender` (records sent messages in memory),
  event handlers, and the `NotificationLog` model.
- **QuickApp.Notification.Tests** — xUnit tests.

## Target architecture
In the monolith `IEmailSender` is called in-process (e.g. `CustomerController`). In the target,
Notification **subscribes** to `OrderPlaced` (from Order) and `UserRegistered` (from Identity)
events carrying only ids + snapshot render data — it never references Order / Customer /
Identity entities.

## Micro-frontend mapping
This is a backend/leaf service with **no dedicated Angular feature area**. Its user-facing
touchpoints live inside other feature areas (e.g. an order-confirmation toast under `orders`,
a welcome message under `login`/`settings`). If surfaced directly it would be a small
"notification preferences / history" micro-frontend rather than a full feature slice.
