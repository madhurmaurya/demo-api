# Product Inventory API (minimal .NET scaffold)

This is a minimal starting scaffold for the Product Inventory API using .NET 7.

Key points:
- Uses JWT bearer tokens for authentication; expects `scope` or `scp` claim with `inventory:read`/`inventory:write`.
- Includes an in-memory repository for local development and CI tests.
- Endpoints:
  - `GET /inventory` - list records (requires `inventory:read`)
  - `GET /inventory/{id}` - get single record (requires `inventory:read`)
  - `POST /inventory` - create/update record (requires `inventory:write`)

To run locally:
```powershell
cd services\product-inventory-api
dotnet run
```

This scaffold is intentionally minimal; production readiness requires JWKS config, secure token validation, persistence, migrations, and CI integration.
