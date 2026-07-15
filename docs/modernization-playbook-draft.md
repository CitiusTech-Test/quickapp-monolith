# Scalable Modernization Orchestration Playbook Draft

## Purpose

Use this playbook to analyze a monolith, propose coarse service boundaries, run parallel additive extraction POCs, and aggregate the results without prematurely rewriting production behavior.

The first calibration target is `CitiusTech-Test/quickapp-monolith`. The closest second validation target is `CitiusTech-Test/ordermanager-monolith`. Legacy-platform migrations such as Oracle Forms, FoxPro and COBOL should reuse the assessment and orchestration gates but supply technology-specific extraction templates.

## Required inputs

- Repository and base branch
- Source and target technology stacks
- Requested service count or candidate business capabilities
- Required project/service naming pattern
- Frontend architecture, if present
- Data stores and migration constraints
- Required build, lint and test commands
- PR target and review policy
- Explicit exclusions such as deployment, database cutover or production rewiring

## Required outputs

1. A decomposition proposal with:
   - current module/data map
   - target service ownership map
   - Mermaid dependency diagram
   - coupling analysis
   - frontend decomposition strategy
   - phased strangler migration
   - manual architecture review flags
2. One self-contained child-session prompt per service.
3. One additive skeleton PR per service.
4. A final summary table containing PR, ownership, validation and coupling flags.
5. A short list of playbook exceptions discovered in the repository.

## Phase A: repository assessment

1. Read repository guidance and setup files.
2. Inventory:
   - deployables and entry points
   - projects/packages/modules
   - entities and database schemas
   - controllers/endpoints and service interfaces
   - scheduled jobs and message handlers
   - UI routes, feature modules and API clients
   - shared libraries and composition roots
3. Trace dependencies from:
   - ORM relationships and cross-schema joins
   - direct service calls
   - shared transactions
   - shared mutable state
   - shared DTO/entity assemblies
   - frontend imports and global stores
4. Record current build/lint/test commands and missing environment prerequisites.

## Phase B: boundary selection

Cluster code by business capability and assign one proposed owner for each mutable data set.

Score each proposed boundary from 0 to 3:

| Dimension | 0 | 1 | 2 | 3 |
| --- | --- | --- | --- | --- |
| Data independence | Same tables/joins | Shared schema | Separate schema possible | Separate store already |
| Transaction coupling | One transaction required | Frequent shared transaction | Compensatable | Independent |
| Runtime dependency | Cyclic/high volume | Frequent synchronous | Limited synchronous | Event/none |
| Change cadence | Always changes together | Often together | Sometimes together | Independent |
| Team/business ownership | Unclear | Shared | Mostly distinct | Explicit |

Use low-scoring boundaries as migration risks, not reasons to hide coupling. Prefer coarse services over entity-per-service decomposition.

## Phase C: proposal and review gates

For every service define:

- owned entities and data
- commands, queries and integration events
- inbound and outbound dependencies
- scalar identifiers and immutable snapshots at boundaries
- data migration/cutover path
- authorization responsibility
- observability and correlation requirements

Human approval is required for:

- service and data ownership
- distributed transaction/saga design
- identity and permission boundaries
- irreversible data cutover
- regulated-data movement
- independent frontend deployment

## Phase D: child-session generation

Write one full prompt file per service. Each prompt must be self-contained because child sessions have isolated filesystems and conversation context.

Every prompt includes:

- repository and PR base
- monolith paths and current behavior
- target responsibilities and owned entities
- exact project/file deliverables
- representative contract operations/events
- anti-coupling rules
- validation commands
- PR completion criteria
- required structured output: PR URL, owned entities, coupling points, validation and blockers

Run children in parallel only when their deliverables do not depend on another child's output. Additive skeletons are independent; production extraction and integration usually are not.

## Phase E: skeleton quality gate

Each service PR must:

- be additive and preserve monolith behavior
- compile independently
- contain contracts, stub implementation and unit tests
- avoid references to another service implementation
- avoid copying cross-domain ORM navigation properties
- use cancellation and nullable/type conventions of the stack
- document deliberately deferred production behavior
- target the requested integration branch

Do not use skeleton code to decide unresolved production semantics. Make ambiguity explicit through local ports and review flags.

## Phase F: aggregation

Verify each PR is open and targets the correct base. Produce:

| Service | PR | Owned entities | External ports/events | Validation | Manual review |
| --- | --- | --- | --- | --- | --- |

Also flag shared-file integration conflicts such as solution files, workspace manifests, lockfiles and central package configuration.

## Phase G: production migration sequence

Default strangler order:

1. Characterization tests and observability
2. In-monolith modular boundaries
3. Event/outbox and contract seams
4. Leaf or stateless services
5. Data-owning services with few dependencies
6. Identity/security authority
7. Orchestrator/transaction-heavy services
8. Frontend route cutover
9. Data reconciliation and monolith retirement

Override the order only with documented dependency and risk evidence.

## Frontend variants

- **Angular/React SPA:** feature folders/libraries first, gateway-relative clients, shell-owned auth, micro-frontends only for real deployment independence.
- **Server-rendered UI:** route/controller modules first, then gateway/BFF composition.
- **Desktop or legacy UI:** introduce API façades and characterization tests before replacing screens.
- **No UI:** omit frontend steps and focus on batch/job/API consumers.

## Technology adapters

The general phases remain stable; project-generation and validation steps are parameters:

- .NET: solution/projects, `dotnet format`, `dotnet build`, `dotnet test`
- Java: Gradle/Maven modules, formatter/static analysis, unit/integration tests
- Python: packages/services, Ruff/type checking, Pytest
- Legacy source: inventory parsers, equivalence fixtures and target-language skeleton templates

## Playbook calibration plan

1. Use this QuickApp run as the baseline example.
2. Run the draft against `CitiusTech-Test/ordermanager-monolith` and record boundary/scaffolding exceptions.
3. Test the assessment gates on one Oracle Forms or FoxPro modernization repo.
4. Parameterize repeated commands and prompt sections; keep architecture decisions as human gates.
5. Convert the reviewed draft into a Devin playbook.
6. Validate the Devin playbook with three tasks: straightforward, cross-domain-coupled and legacy-technology-heavy.
