# POS System — ERD & UML Report

**Status:** Draft  
**Author:** Kenneth Rey Tablang  
**Date:** 2025-08-27

---

## Purpose

This document explains the Entity Relationship Diagram (ERD) for the Point of Sale (POS) system, summarizes design choices, and points to the UML artifacts (class diagrams, sequence diagrams, activity diagrams) used for deeper design documentation. Use the links below to open the live diagrams (Mermaid Live, PlantUML, or your diagram hosting service).

---

## Links (paste your live links here)

- **ER Diagram (Mermaid Live):** `<https://www.mermaidchart.com/app/projects/c70298c0-9143-4e2a-be33-ddf5f37be52f/diagrams/e670b369-e66a-4e90-890c-c11872abed9a/version/v0.1/edit>`
- **UML — Class Diagram:** `<UML_CLASS_LINK>`
- **UML — Sequence Diagram: Create & Print Sale:** `<UML_SEQ_SALE_PRINT_LINK>`
- **UML — Sequence Diagram: Receive Purchase Order:** `<UML_SEQ_RECEIVE_PO_LINK>`
- **UML — Activity Diagram: End-of-Day (Z Reading):** `<UML_ACTIVITY_ZREAD_LINK>`

> Tip: Put these URLs in `README.md` or `TECHNICAL_DOCUMENTATION.md` using the link examples below.

---

## Executive Summary

The ERD models the core domain for a retail POS: users/roles, products/inventory, suppliers/purchases, sales/payments, and printing/BIR compliance. The model favors:

- **Auditability** (SystemLog, ReceiptLog, ReprintLog)
- **Traceable inventory movements** (InventoryTransaction, ReceivedStock, PurchaseItem)
- **BIR-compliant printing** (InvoiceCounter, ReceiptLog)
- **Extensibility** for features like price tiers, customers, multi-location, and serialized stock

Key design goals:
- Avoid destructive deletes (use soft-delete flags) to preserve historical references for audits and reports.
- Keep sale processing transactional (ACID) to ensure inventory and financial integrity.
- Centralize business rules in service layer (e.g., `ReceivedStock` -> create `InventoryTransaction`).

---

## Key Entities & Relationships (high level)

### Core domain entities
- **ApplicationUser** — system user (Admin, Manager, Cashier, Warehouse)
- **UserRole** — user roles with permission boundaries
- **Product** — sellable item (barcode, price, cost, tax type, unit)
- **UnitConversion** — multi-unit support (box → piece)
- **InventoryTransaction** — atomic stock movements (StockIn, Sale, Return, Adjustment)
- **Supplier / PurchaseOrder / PurchaseItem / ReceivedStock** — purchasing & receiving workflow
- **Sale / SaleItem / Payment** — sales transactions and payment capture
- **ReceiptLog / InvoiceCounter / ReprintLog** — printing and legal receipt tracking
- **SystemLog / LoginAttemptLog** — audit and security trails
- **StockAdjustment / BadOrder** — corrections and waste tracking

### Important relationships & cardinalities
- `Product` 1 — * `SaleItem` (a product can appear many times in sale items)
- `Sale` 1 — * `SaleItem` and 1 — * `Payment`
- `PurchaseOrder` 1 — * `PurchaseItem`
- `ReceivedStock` 1 — * `InventoryTransaction` (each receive should create stock-in transactions)
- `ApplicationUser` 1 — * `Sale`, `PurchaseOrder`, `ReceivedStock`, `SystemLog` (actors)

---

## Design Rationale & Notes

### Normalization & Data Integrity
- The data model is **3NF-friendly**: core attributes stored on entities; junction or history data (SaleItem, InventoryTransaction) stored separately to avoid repetition and capture historical states.
- Use **foreign keys** for referential integrity but consider **soft-delete** patterns for business objects that must remain auditable (e.g., Product, User, Sale).

### Transactions & Concurrency
- Sale processing should be wrapped in a **database transaction**: create Sale → SaleItems → Payment(s) → InventoryTransactions → ReceiptLog.
- Use optimistic concurrency tokens (`RowVersion`/`Timestamp`) on mutable reference tables (Product stock/cost) to handle concurrent access at registers.

### Indexing & Performance
- Recommended indexes:
  - `Product(Barcode)`, `Product(Name)` for lookups.
  - `InventoryTransaction(ProductId, Timestamp)` for stock history queries.
  - `Sale(SaleDate)`, `ReceiptLog(InvoiceNumber)` for reporting and fast lookup.
  - `PurchaseOrder(SupplierId, CreatedAt)` for vendor reports.
- Consider read-optimized summary tables (materialized views) for heavy reports (TopSellingProductLog, CashierSalesSummary).

### Auditability & Retention
- Store `ReceiptLog` (receipt copy) and `ReprintLog` for BIR audits. Reprints must capture `requestedBy`, `reason`, and timestamp.
- `SystemLog` should be append-only and retain events for a retention period aligned with business / legal requirements. Archive older logs to cold storage.

### Soft Deletes & Referential Behavior
- Soft-delete `IsActive` or `IsDeleted` flags for Product, User, Supplier, PurchaseOrder to avoid cascade deletions breaking historical data.
- Cascade delete only for truly ephemeral data (e.g., temp session records), not for financial or audit records.

### Security
- Secure sensitive endpoints (printing, reprint approval, void sale) with role checks and require manager approval for sensitive operations.
- Protect endpoints with JWT and enable logging for suspicious activities (LoginAttemptLog).

---

## Recommended UML Diagrams (what to include & why)

1. **Class Diagram**
   - Shows domain models (Product, Sale, PurchaseOrder, InventoryTransaction, ReceiptLog).
   - Useful for backend model + DTO alignment and AutoMapper profiles.

2. **Sequence Diagrams**
   - **Create & Print Sale**: Frontend → API → SaleService → InventoryService → ReceiptService → Print Service/Terminal.
   - **Receive Purchase Order**: Warehouse UI → API → PurchaseService → InventoryTransaction creation → Update Product stock.

3. **Activity Diagrams**
   - **Z Reading (End-of-Day)**: show aggregation of transactions, generating the Z reading, resetting counters, and archiving reports.

4. **Deployment Diagram** (optional)
   - Show backend, DB, POS terminals, printers, and local network topology for installation documentation.

---

## How to embed the links in Markdown

**Inline link example**
```md
[ER Diagram (Mermaid Live)](<ERD_LINK>)
