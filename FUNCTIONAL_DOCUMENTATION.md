# POS System — Functional Documentation

> Definitions first: each module begins with concept/definition, then data models, then API contract examples (DTOs, endpoints, sample requests/responses).

## Table of Contents

1. Auth & Users
2. Products & Inventory
3. Sales & Payments
4. Suppliers & Purchasing
5. Settings & Readings (X/Z)
6. Printing & BIR Compliance
7. System Logging & Audit
8. Error Handling & Status Codes

---

## 1. Authentication & Users

### Definition

Authentication provides secure access. Authorization controls behavior per role. Users can be created, updated, and deactivated by Admins.

### Core Models (summary)

* `ApplicationUser`: Id, Username, Email, FullName, Role, IsActive, CreatedAt
* `UserRole` (enum): Admin, Manager, Cashier, Warehouse

### DTOs (examples)

```ts
// UserCreateDto
{
  "userName": "johndoe",
  "email": "john@store.local",
  "fullName": "John Doe",
  "password": "P@ssw0rd!",
  "role": "Cashier"
}

// UserReadDto
{
  "id": "...",
  "userName": "johndoe",
  "email": "john@store.local",
  "fullName": "John Doe",
  "role": "Cashier",
  "isActive": true
}
```

### Controller Endpoints (examples)

```
POST /api/account/login
POST /api/users
GET  /api/users
GET  /api/users/{id}
PUT  /api/users/{id}
PATCH /api/users/{id}/deactivate
```

### Sample: Create user (curl)

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"userName":"cash1","email":"cash1@store.local","fullName":"Cashier 1","password":"P@ss1","role":"Cashier"}'
```

Notes:

* Passwords must follow policy (min length, complexity).
* Deactivation keeps historical user references intact (sales references user id).

---

## 2. Products & Inventory

### Definition

Products are sellable items. Inventory keeps track of physical stock by location. Multi-unit support allows purchasing in one unit (e.g., box) and selling in another (piece).

### Core Models (summary)

* `Product`: Id, Name, Barcode, Price, Cost, Unit, Stock, ReorderLevel, TaxType
* `UnitConversion`: ProductId, FromUnit, ToUnit, Factor (e.g., 1 box = 24 pieces)
* `InventoryTransaction`: Id, ProductId, Quantity, ActionType, ReferenceId, Timestamp, UserId

### Business Rules

* When `ReceivedStock` is added, an `InventoryTransaction` with `ActionType = StockIn` must be created automatically (service layer responsibility).
* Sales decrement stock via `InventoryTransaction` with `ActionType = Sale`.
* Stock adjustments create `InventoryTransaction` with `ActionType = Adjustment` and require a reason.

### DTO Examples

```ts
// ProductCreateDto
{
  "name": "Canned Tuna",
  "barcode": "1234567890123",
  "price": 55.00,
  "cost": 40.00,
  "unit": "piece",
  "reorderLevel": 10,
  "taxType": "VATABLE"
}
```

### Endpoints (examples)

```
GET  /api/products
GET  /api/products/{id}
POST /api/products
PUT  /api/products/{id}
DELETE /api/products/{id} (soft-delete)
POST /api/products/{id}/unit-conversion
POST /api/stock/receive
POST /api/stock/adjust
```

### Sample: Receive stock payload

```json
{
  "purchaseOrderId": 12,
  "receivedAt": "2025-08-25T09:45:00",
  "items": [
    { "productId": 101, "quantity": 5, "unit": "box", "convertedTo": "piece", "convertedQuantity": 120 }
  ]
}
```

Notes:

* Keep `convertedQuantity` stored to simplify stock math.
* Unit conversions should be validated at service layer.

---

## 3. Sales & Payments

### Definition

A `Sale` is a transaction containing one or more `SaleItem`s and zero or more `Payment`s. Sales should be atomic and produce a printable receipt. Sale processing must update inventory and create `ReceiptLog` entries.

### Core Models

* `Sale`: Id, SaleDate, SubTotal, TaxAmount, DiscountAmount, TotalAmount, CashierId, Status
* `SaleItem`: Id, SaleId, ProductId, Quantity, UnitPrice, DiscountAmount, TotalPrice
* `Payment`: Id, SaleId, PaymentType, Amount, TenderedAmount, Change

### DTO Example: Create Sale

```json
{
  "items": [
    { "productId": 101, "quantity": 2, "unitPrice": 55.00 },
    { "productId": 102, "quantity": 1, "unitPrice": 120.00 }
  ],
  "payments": [
    { "paymentType": "Cash", "amount": 230.00 }
  ],
  "cashierId": "user-123",
  "applyDiscounts": true
}
```

### Endpoints (examples)

```
POST /api/sales
GET  /api/sales/{id}
GET  /api/sales?dateFrom=...&dateTo=...
POST /api/sales/{id}/void
POST /api/sales/{id}/return
```

### Important Behaviour

* Sales processing must be transactional: create Sale → create SaleItems → create Payments → update inventory → create ReceiptLog. If any step fails, rollback.
* For offline/local network setups, consider durable queueing for receipts or background tasks that reconcile with central DB.

### Example: Minimal payment flow (curl)

```bash
curl -X POST http://localhost:5000/api/sales \
  -H "Content-Type: application/json" \
  -d '{"items":[{"productId":101,"quantity":2,"unitPrice":55}],"payments":[{"paymentType":"Cash","amount":110}],"cashierId":"user-123"}'
```

---

## 4. Suppliers & Purchasing

### Definition

Supplier module manages vendors and purchase orders (PO). POs generate expected stock receipts.

### Models & DTOs

* `Supplier`: Id, Name, Contact, PaymentTerms
* `PurchaseOrder`: Id, SupplierId, CreatedBy, Items\[], Status (Pending, PartiallyReceived, Completed)
* `PurchaseItem`: ProductId, OrderedQuantity, ReceivedQuantity, UnitCost

### Endpoints

```
POST /api/suppliers
GET  /api/suppliers
POST /api/purchase-orders
GET  /api/purchase-orders/{id}
POST /api/purchase-orders/{id}/receive
```

Notes:

* Receiving against a PO should update PO status and create `ReceivedStock` + `InventoryTransaction`.

---

## 5. Settings & Readings

### Definition

Settings control VAT, Discount behaviors, Receipt formatting. X/Z Readings are shift or day-end summaries required for audit/BIR.

### Key Models

* `VatSetting`, `ReceiptSetting`, `DiscountSetting`
* `XReading`: intermediate reading (no reset)
* `ZReading`: final daily reading (resets day counters)

### Endpoints

```
GET  /api/settings/receipt
PUT  /api/settings/receipt
POST /api/readings/x
POST /api/readings/z
GET  /api/readings/z?date=2025-08-25
```

Notes:

* ZReading generation should capture totals by payment type, VAT breakdown, and number of receipts.

---

## 6. Printing & BIR Compliance

### Definition

Printing subsystem produces receipts/invoices following BIR requirements — unique sequential invoice numbers, reprint logs, and restricted reprint actions.

### Core Models

* `InvoiceCounter`: current numeric counter per terminal
* `SerialNumberTracker`: tracks serials for printed receipts
* `ReprintLog`: records reprint metadata (who, when, reason)
* `ReceiptLog`: stores printed receipt copy (for audit)

### Business Rules

* Each printed official receipt must use the next number from `InvoiceCounter` for that terminal.
* Reprints must increment a reprint counter and log reason.
* Reprinting official receipt might require manager approval depending on business rules.

### Endpoints

```
POST /api/print/receipt (body: saleId, terminalId)
POST /api/print/reprint (body: saleId, reason, requestedBy)
GET  /api/receipts/{saleId}
```

### Example: Reprint payload

```json
{
  "saleId": 555,
  "reason": "Customer requested duplicate",
  "requestedBy": "manager-001"
}
```

Notes:

* Keep copies of printed receipts in `ReceiptLog` to reconstruct any day’s receipts for audits.

---

## 7. System Logging & Audit

### Definition

A tamper-evident trail of important actions — login attempts, user changes, sale voids, stock adjustments.

### Core Models

* `SystemLog`: EventType, ActorUserId, Description, Timestamp, ReferenceId
* `LoginAttemptLog`: Username, Success, IP, Timestamp

### Examples of events to log

* User created/updated/deactivated
* Sale created/voided/returned
* Stock adjustments and reasons
* Price changes

### Endpoints

```
GET /api/logs?type=Sale&dateFrom=...
```

Notes:

* Logs are append-only; soft-delete or archiving is allowed for retention policies.

---

## 8. Error Handling & Status Codes

### Principles

* Use appropriate HTTP status codes:

  * `200 OK`: successful GET/PUT
  * `201 Created`: POST created resource
  * `204 No Content`: successful action without body
  * `400 Bad Request`: validation errors
  * `401 Unauthorized`: auth required
  * `403 Forbidden`: insufficient role/permission
  * `404 Not Found`: missing resource
  * `409 Conflict`: concurrency or business rule conflict
  * `500 Internal Server Error`: unexpected

### Error response structure (example)

```json
{
  "status": 400,
  "errors": [
    {"field": "email", "message": "Email is required"}
  ],
  "traceId": "..."
}
```

---

## Enums & Shared Types (summary)

```ts
// TaxType
enum TaxType { VATABLE = 'VATABLE', EXEMPT = 'EXEMPT', ZERO_RATED = 'ZERO_RATED' }

// InventoryActionType
enum InventoryActionType { StockIn, Sale, Return, Void, Adjustment, BadOrder }

// UserRole
enum UserRole { Admin, Manager, Cashier, Warehouse }
```

---

## Appendix — Example Workflows

### A. Create and print a Sale (flow)

1. Cashier submits Sale DTO to `POST /api/sales`.
2. Backend validates prices, stock, and discounts.
3. Create `Sale`, `SaleItem`s, and `Payment` records inside a transaction.
4. For each sold item, create `InventoryTransaction` with `ActionType = Sale`.
5. Decrement Product stock.
6. Generate ReceiptNumber using `InvoiceCounter` for terminal.
7. Create `ReceiptLog` and trigger printing job.
8. Return receipt data to frontend for immediate printing and local copy.

### B. Receive purchase order and update stock

1. Warehouse receives goods and sends `POST /api/purchase-orders/{id}/receive` with items.
2. Backend creates `ReceivedStock` records.
3. For each received item, create `InventoryTransaction` with `ActionType = StockIn`.
4. Update product stock levels.
5. If received quantity < ordered, mark PO as `PartiallyReceived`.

---

## Contact / Ownership

* Maintainer: Kenneth Rey Tablang
* Repo: Add your git remote URL here

---

*End of functional documentation.*
