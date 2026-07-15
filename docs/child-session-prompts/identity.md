# Identity service extraction session

Work in repository **CitiusTech-Test/quickapp-monolith** on Linux. Open a PR targeting the existing **`modernization-poc`** branch.

## Objective

Extract the Identity domain into a standalone, additive project skeleton. Do not remove or rewire the monolith. Create:

- `QuickApp.Identity.Contracts`
- `QuickApp.Identity.Service`
- `QuickApp.Identity.Service.Tests`

## Monolith context

- The solution targets `net10.0`.
- Identity entities are in `QuickApp.Core/Models/Account`: `ApplicationUser`, `ApplicationRole`, `ApplicationPermission`.
- Identity services are in `QuickApp.Core/Services/Account`.
- Token issuance is in `QuickApp.Server/Controllers/AuthorizationController.cs` using ASP.NET Identity and OpenIddict.
- User and role APIs are in `UserAccountController` and `UserRoleController`.
- Policies and handlers are in `QuickApp.Server/Authorization`.
- `ApplicationDbContext` currently mixes Identity/OpenIddict tables with Customer, Product and Order tables.
- `ApplicationUser.Orders` and `Order.Cashier` currently create a database-level dependency between Identity and Order.
- The Angular Identity surface is login plus user/role/settings controls and the account/auth services.

## Target responsibilities and ownership

Identity owns users, credentials, roles, claims, permissions, OpenIddict clients/tokens and token issuance. It publishes stable user identifiers and claims but does not own orders, customers, products or notifications.

Use string user/role IDs to preserve compatibility with ASP.NET Identity. Other services must treat user IDs as opaque scalar values.

## Required contract shape

Create clear contracts for representative operations such as:

- get user by ID
- list users
- list roles/permissions
- register or update a user without exposing persistence entities
- a `UserRegistered` integration event

Do not place password hashes, security stamps, refresh tokens or framework Identity entities in the Contracts project. Authentication implementation can remain stubbed; do not invent a new credential mechanism.

## Implementation requirements

- Follow repository conventions and `ai-rules/AI_RULES.md` where applicable.
- `QuickApp.Identity.Contracts` must be a plain class library with DTO/record and interface definitions.
- `QuickApp.Identity.Service` must reference Contracts and contain a compilable stub implementation with deterministic, safe placeholder behavior.
- Add a minimal service registration/composition entry point appropriate for a standalone service skeleton.
- Add `QuickApp.Identity.Service.Tests` with passing unit tests for the stub and important contract behavior.
- Add all projects to `QuickApp.sln`.
- Use nullable reference types and async APIs with cancellation tokens.
- Keep the change focused; no production database migration, OAuth cutover, Angular rewrite, containers or deployment manifests.

## Coupling rules

- Do not reference `QuickApp.Core`, `QuickApp.Server`, Shop models, or another extracted service implementation.
- Do not copy `ApplicationUser.Orders` or any Order navigation into Identity.
- Do not create contracts that require callers to understand ASP.NET Identity persistence types.
- Do not make Notification a direct dependency. `UserRegistered` is the future integration seam.
- Do not alter existing monolith authentication or authorization behavior.

## Validation and PR

- Run applicable format, build and unit-test commands. If the .NET 10 SDK is absent, install it non-destructively if practical; otherwise document the exact validation limitation in the PR.
- Review the merge-base diff for unrelated changes.
- Commit and push a dedicated branch.
- Open a non-draft PR into `modernization-poc`.
- PR summary must identify owned entities and the remaining coupling points: `Order.Cashier`, shared `ApplicationDbContext`, audit user claims and centralized permission registration.

Done when the three projects, interfaces/contracts, stub implementation, passing unit tests where tooling permits, and PR are present.
