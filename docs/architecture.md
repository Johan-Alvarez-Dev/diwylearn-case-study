# Public Architecture

## Backend shape

- `Domain`: courses, modules, sections, lessons, enrollment, and progress.
- `Data`: EF Core DbContext, configurations, indexes, and migrations.
- `Features`: controller, contracts, mappings, and service per capability.
- `Infrastructure`: Identity, JWT, sanitization, and external adapters.
- `Common`: roles and consistent API error contracts.

Entities are never returned directly as HTTP responses. Read models use `Select` projections and `AsNoTracking`; ownership checks remain server-side.

## Authorization flow

1. JWT resolves user identity and roles.
2. Endpoint policy verifies the coarse role.
3. Application service verifies ownership or enrollment.
4. Query projects only authorized data.
5. Explicit responses distinguish invalid input, forbidden access, and missing resources.

## Frontend shape

React is organized by feature. Course Studio and the student player share block contracts and rendering shells. Persisted JSON is parsed defensively and invalid state fails safely.
