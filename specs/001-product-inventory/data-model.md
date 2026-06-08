# Data Model

## Entities

### Product

Represents a catalog item with inventory information.

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `productId` | string | Primary Key, non-empty, unique | Unique identifier for the product (e.g., SKU) |
| `name` | string | Required, non-empty | Product name or title |
| `description` | string | Optional | Detailed product description |
| `quantity` | integer | >= 0 | Quantity on hand; must be non-negative |
| `lastUpdated` | ISO 8601 timestamp | Auto-set | Timestamp of last inventory update |

**Validation Rules**:

- `productId` must not be empty or contain only whitespace.
- `quantity` must be an integer in range [0, max-safe-integer].
- `quantity` must never be updated to a negative value; requests with negative quantities are rejected with HTTP 400.
- `lastUpdated` is set by the system on create and update; client-provided values are ignored.

**State Transitions**:

- **Create**: New product added to inventory. Quantity defaults to provided value or 0.
- **Update**: Existing product's quantity is replaced. Quantity must pass validation. On update, `lastUpdated` is refreshed.
- **List**: All products are returned in a deterministic order (e.g., by `productId` ascending).

---

## Relationships

- **Product ↔ InventoryRecord**: In this design, they are unified into a single entity. The `Product` represents both catalog and inventory.
- No foreign key relationships are anticipated for MVP.

---

## Key Constraints

1. **Non-Negative Quantity**: The system enforces non-negative inventory at all times.
   - Database schema or validation must prevent negative quantities.
   - API validation rejects requests with quantity < 0 before persistence.

2. **Unique Product Identifier**: Each `productId` is unique within the inventory.
   - No duplicate `productId` values are allowed.
   - Updates are keyed by `productId`.

3. **Immutable Audit Trail**: `lastUpdated` timestamp records when inventory was last changed.
   - This value is managed by the system, not the client.
   - Enables debugging and audit logging.

---

## Assumptions

- Products do not have parent-child or hierarchical relationships in this MVP.
- No multi-tenant or organization scoping is required.
- All products share the same set of fields; no product variants or SKU hierarchies.
