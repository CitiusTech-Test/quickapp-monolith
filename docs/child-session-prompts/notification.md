# Notification service extraction session

Work in repository **CitiusTech-Test/quickapp-monolith** on Linux. Open a PR targeting the existing **`modernization-poc`** branch.

## Objective

Extract the Notification domain into a standalone, additive project skeleton. Do not remove or rewire the monolith. Create:

- `QuickApp.Notification.Contracts`
- `QuickApp.Notification.Service`
- `QuickApp.Notification.Service.Tests`

## Monolith context

- The solution targets `net10.0`.
- Backend email is represented by `QuickApp.Core/Services/IEmailSender.cs` and `QuickApp.Server/Services/Email/EmailSender.cs`.
- SMTP settings and templates are configured by the shared server host.
- `CustomerController` directly invokes `IEmailSender`, demonstrating synchronous business-to-delivery coupling.
- Angular has `NotificationService`, `NotificationEndpoint`, `notification.model` and a notification viewer.
- `NotificationEndpoint` contains in-memory demo notifications; there is no backend notification source of truth.
- No durable delivery request, retry, preference or delivery-log entity currently exists.

## Target responsibilities and ownership

Notification owns outbound delivery requests, channel selection, templates and optional durable delivery status/in-app notification records. It consumes integration events from other services and translates them into messages.

For the POC, keep the service transport-agnostic and use a deterministic in-memory/stub sender. Do not move real SMTP credentials or production templates.

## Required contract shape

Create clear contracts for representative operations such as:

- submit a notification request
- query delivery status
- channel and recipient models
- delivery result/status

The contract may carry source event type, correlation ID and idempotency key. It must not reference Customer, Product, Order or Identity entities.

## Implementation requirements

- Follow repository conventions and `ai-rules/AI_RULES.md` where applicable.
- `QuickApp.Notification.Contracts` must be a plain class library with DTO/record and interface definitions.
- `QuickApp.Notification.Service` must reference Contracts and contain a compilable deterministic stub implementation.
- Add a minimal service registration/composition entry point appropriate for a standalone service skeleton.
- Add `QuickApp.Notification.Service.Tests` with passing unit tests for submission, status and idempotent stub behavior.
- Add all projects to `QuickApp.sln`.
- Use nullable reference types and async APIs with cancellation tokens.
- Keep the change focused; no SMTP credential migration, real broker, Angular rewrite, database migration, containers or deployment manifests.

## Coupling rules

- Do not reference `QuickApp.Core`, `QuickApp.Server` or any other service implementation.
- Do not depend on another domain's event assembly; use Notification-owned ingestion/request contracts for the skeleton.
- Do not copy secrets or production SMTP configuration.
- Do not make another service call Notification synchronously in the monolith.
- Do not treat Angular demo notification data as authoritative domain data.
- Do not modify existing email or Angular notification behavior.

## Validation and PR

- Run applicable format, build and unit-test commands. If the .NET 10 SDK is absent, install it non-destructively if practical; otherwise document the exact validation limitation in the PR.
- Review the merge-base diff for unrelated changes.
- Commit and push a dedicated branch.
- Open a non-draft PR into `modernization-poc`.
- PR summary must identify delivery requests/status/templates as owned and flag SMTP adaptation, durable retries, user preferences, event schema ownership and replacement of Angular demo data.

Done when the three projects, interfaces/contracts, stub implementation, passing unit tests where tooling permits, and PR are present.
