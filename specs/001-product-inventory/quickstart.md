# Quickstart & Validation Guide

## Overview

This guide walks you through running the Product Inventory API locally, obtaining a JWT token for authentication, and validating the core endpoints.

## Prerequisites

- .NET 8 SDK installed
- `curl` or Postman for testing (or use Swagger UI)
- Port 5001 available for the local API

## Running the API Locally

### Step 1: Start the API in Development Mode

```powershell
cd services/product-inventory-api
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --urls=http://localhost:5001
```

**Expected output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 2: Verify Swagger UI

Open your browser and navigate to:

```
http://localhost:5001/swagger/index.html
```

You should see an interactive API documentation page listing all inventory endpoints.

---

## Obtaining a Development JWT Token

### Using the Dev Token Endpoint

In development mode, a `/dev/token` endpoint is available to mint temporary JWT tokens for testing.

**Request**:

```bash
curl -X POST http://localhost:5001/dev/token \
  -H "Content-Type: application/json" \
  -d '{
    "audience": "product-inventory-api",
    "scopes": ["inventory:read", "inventory:write"],
    "expiresHours": 1
  }'
```

**Expected response**:

```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

**Copy the `access_token` value for use in subsequent requests.**

---

## Validating Core Endpoints

### Test 1: List Inventory (GET /inventory)

```bash
TOKEN="<paste your access_token here>"

curl -X GET http://localhost:5001/inventory \
  -H "Authorization: Bearer $TOKEN"
```

**Expected response** (HTTP 200):

```json
[
  {
    "productId": "PROD-001",
    "name": "Widget A",
    "description": "Standard widget",
    "quantity": 50,
    "lastUpdated": "2026-06-08T10:30:00Z"
  }
]
```

**Without token** (HTTP 401):

```
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

---

### Test 2: Get Single Product (GET /inventory/{id})

```bash
curl -X GET http://localhost:5001/inventory/PROD-001 \
  -H "Authorization: Bearer $TOKEN"
```

**Expected response** (HTTP 200):

```json
{
  "productId": "PROD-001",
  "name": "Widget A",
  "description": "Standard widget",
  "quantity": 50,
  "lastUpdated": "2026-06-08T10:30:00Z"
}
```

**For a product that doesn't exist** (HTTP 404):

```json
{
  "error": "NOT_FOUND",
  "message": "Product not found"
}
```

---

### Test 3: Create or Update Inventory (POST /inventory)

```bash
curl -X POST http://localhost:5001/inventory \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "PROD-002",
    "name": "Widget B",
    "description": "Premium widget",
    "quantity": 100
  }'
```

**Expected response** (HTTP 200):

```json
{
  "productId": "PROD-002",
  "name": "Widget B",
  "description": "Premium widget",
  "quantity": 100,
  "lastUpdated": "2026-06-08T11:05:00Z"
}
```

**Invalid quantity (negative)** (HTTP 400):

```bash
curl -X POST http://localhost:5001/inventory \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "PROD-003",
    "quantity": -5
  }'
```

**Expected response** (HTTP 400):

```json
{
  "error": "VALIDATION_ERROR",
  "message": "Quantity must be non-negative",
  "details": {
    "quantity": "Quantity must be non-negative"
  }
}
```

---

### Test 4: Authorization Scopes

**Token with read-only scope**:

```bash
curl -X POST http://localhost:5001/dev/token \
  -H "Content-Type: application/json" \
  -d '{
    "audience": "product-inventory-api",
    "scopes": ["inventory:read"],
    "expiresHours": 1
  }'
```

**Try to POST with read-only token** (HTTP 403):

```bash
curl -X POST http://localhost:5001/inventory \
  -H "Authorization: Bearer <read-only-token>" \
  -H "Content-Type: application/json" \
  -d '{"productId": "PROD-004", "quantity": 50}'
```

**Expected response** (HTTP 403):

```json
{
  "error": "FORBIDDEN",
  "message": "Insufficient permissions; required scope not present"
}
```

---

## Using Swagger UI to Test

1. Open `http://localhost:5001/swagger/index.html` in your browser.
2. Click the **Authorize** button (top right).
3. Paste your JWT token (without the "Bearer " prefix) into the token field.
4. Click **Authorize**.
5. Click on an endpoint (e.g., `GET /inventory`).
6. Click **Try it out** and then **Execute**.

---

## References

- [Data Model](./data-model.md) — Entity definitions and validation rules
- [OpenAPI Contract](./contracts/openapi.yaml) — Full API specification
- [API Plan](./plan.md) — Implementation strategy and milestones
