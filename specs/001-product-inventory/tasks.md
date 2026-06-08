# Tasks: Product Inventory API

## Overview
This file breaks the implementation plan into ordered, actionable tasks with rough estimates (hours) and suggested priority.

### Legend
- Est: estimated hours (rough)
- Prio: P0 (highest) → P2 (lower)

## Task List

1. Define OpenAPI contract (Est: 6h, Prio: P0)
   - Create `openapi.yaml` with models and endpoints
   - Define auth scheme (OAuth2/JWT) and scopes (`inventory:read`, `inventory:write`)

2. Create project scaffolding & CI (Est: 4h, Prio: P0)
   - Minimal project layout, linting, and CI job skeleton
   - Add coverage and test steps

3. Implement persistence interface & schema (Est: 8h, Prio: P0)
   - Migration/schema for `products` and `inventory_records`
   - Repository interface + in-memory fallback for local dev

4. Implement service layer (Est: 10h, Prio: P0)
   - Business rules, quantity constraints, concurrency considerations
   - Unit tests for service logic

5. Implement auth middleware (Est: 6h, Prio: P0)
   - JWT validation, JWKS config, scope enforcement middleware
   - Tests for 401/403 cases

6. Implement API endpoints (Est: 8h, Prio: P0)
   - `GET /inventory`, `GET /inventory/{id}`, `POST /inventory`/`PUT /inventory/{id}`
   - Input validation and response formatting

7. Validation & structured errors (Est: 4h, Prio: P1)
   - Centralized validation, error shapes, mapping to 4xx codes

8. Integration tests (Est: 8h, Prio: P0)
   - HTTP-level tests including token scenarios and DB interactions

9. Documentation & OpenAPI hosting (Est: 3h, Prio: P1)
   - Swagger UI or ReDoc integration and examples

10. Observability & health checks (Est: 3h, Prio: P2)
    - `/health`, basic metrics and logging hooks

11. Final polish & release (Est: 4h, Prio: P1)
    - Ensure coverage >=80%, update README, merge PR checklist

## Task Owners & Notes
- Start with `openapi` and `auth` in parallel to reduce rework.
- Use in-memory DB for local dev and CI; add production DB migrations as separate PR.
- Mock or configure a JWKS endpoint for integration tests.

## Next actions
- Pick implementation stack and create the initial repository scaffolding (I can do this if you confirm the stack).
