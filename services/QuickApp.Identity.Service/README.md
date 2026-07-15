# QuickApp.Identity.Service

Standalone skeleton for the **Identity** microservice extracted from the QuickApp
monolith as part of the decomposition POC (see `docs/decomposition-proposal.md` on
the `modernization-poc` branch).

## What it owns

- `ApplicationUser`, `ApplicationRole`, and the ASP.NET Identity claim/role concepts
  (represented here as plain POCOs).
- The **permission catalog** (`ApplicationPermissions`), ported as plain
  `PermissionDto` constants.
- It is the intended OAuth2/OIDC **token issuer** for the whole system.

## Projects

- `QuickApp.Identity.Contracts` — dependency-free interfaces (`IUserAccountService`,
  `IUserRoleService`, `IPermissionCatalog`) and DTOs (`UserDto`, `RoleDto`,
  `PermissionDto`, `UserWithRolesDto`, `OperationResult`).
- `QuickApp.Identity.Service` — in-memory (dictionary-backed) stub implementation.
- `QuickApp.Identity.Tests` — xUnit tests against the stub.

## Frontend mapping (micro-frontend note)

The Angular `login` and `settings` (users/roles management) feature areas map to this
service. As a micro-frontend it would become the **Identity/settings remote** owning
the login screen and the user/role administration screens, with token acquisition and
refresh living in the shell so every other remote can obtain and validate JWTs issued
by this service.

## Coupling broken during extraction

See the PR description for the full list. In short: the user is referenced by other
services only by **id** (e.g. `Order.CashierUserId`); this service does not depend on
any Shop entity, and it becomes the sole JWT issuer while the other services act as
resource servers validating those tokens.
