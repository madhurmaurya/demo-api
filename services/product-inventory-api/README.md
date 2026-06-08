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

Configuration
 - Configure JWT settings via `appsettings.json` or environment variables under `Jwt`:
   - `Authority` — optional OIDC authority (preferred). If set, the middleware will use the authority metadata to fetch signing keys.
   - `Audience` — expected token audience.
   - `JwksUri` — optional direct JWKS URL to fetch signing keys if not using an OIDC authority.

Example development config: `appsettings.Development.json` is included with placeholders.

Local Token Generator (development only)

For local testing you can mint a JWT using the development token endpoint (only available in the `Development` environment):

POST /dev/token

Request body (JSON):
{
  "sub": "test-user",
  "scopes": ["inventory:read","inventory:write"],
  "audience": "product-inventory-api",
  "expiresHours": 1
}

Response:
{
  "access_token": "...",
  "token_type": "Bearer",
  "expires_in": 3600
}

Use the returned `access_token` in `Authorization: Bearer <token>` header to call protected endpoints.

Note: The dev token endpoint reads `Jwt:DevSigningKey` from configuration. Do not use this key in production.
