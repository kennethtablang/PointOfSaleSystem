# Point of Sale (POS) System

> Lightweight POS system for SMEs (grocery, convenience, small retail). Built to be local-network friendly and BIR-compliant.

## Overview

This repository contains two main parts:

* **Backend**: ASP.NET Core Web API (C#) — business logic, data access, authentication, and BIR-compliant receipt generation.
* **Frontend**: React + Vite + TypeScript + Tailwind + DaisyUI — cashier UI, admin dashboards, and management screens.

Key goals:

* Reliable sales processing (barcode & non-barcode)
* Robust inventory (multi-unit, adjustments, reorder alerts)
* Clear separation: Models → DTOs → AutoMapper → Interfaces → Services → Controllers → Frontend
* Auditability and BIR (receipt numbering, reprint logs, Z/X readings)

## Tech Stack

* Backend: .NET 8 / ASP.NET Core Web API, Entity Framework Core, SQL Server
* Frontend: React, Vite, TypeScript, TailwindCSS, DaisyUI
* State & Tools: React Query (TanStack), Zustand, React Hook Form, Yup, Axios
* Dev: Docker (optional), EF Core Migrations, Git

## Quickstart

### Prerequisites

* .NET 8 SDK
* Node.js 18+
* SQL Server (local or container)
* Optional: Docker

### Backend

```bash
cd backend
# restore
dotnet restore
# apply migrations
dotnet ef database update
# run API
dotnet run
```

### Frontend

```bash
cd pos-frontend
npm install
npm run dev
```

## Repo Structure (high-level)

```
root/
├─ backend/
│  ├─ Controllers/
│  ├─ Data/ (DbContext, Migrations)
│  ├─ DTOs/
│  ├─ Interfaces/
│  ├─ Mappings/ (AutoMapper)
│  ├─ Models/
│  ├─ Services/
│  └─ Program.cs
└─ pos-frontend/
   ├─ src/
   │  ├─ features/ (auth, sales, inventory, products, reports, settings)
   │  ├─ components/
   │  ├─ layouts/
   │  └─ services/ (axiosInstance, authService, productService)
   └─ package.json
```

## Environment Variables (examples)

```
# Backend - appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pos_db;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "YourVeryLongSecretKeyHere",
    "Issuer": "pos.local",
    "Audience": "pos.clients"
  }
}

# Frontend - .env
VITE_API_BASE_URL=http://localhost:5000/api
```

## Running in Docker (optional)

Add Dockerfile for backend and frontend, and a docker-compose.yml to tie together SQL Server, backend, and frontend. Keep the database service accessible via a network alias for migrations.

## Features (short)

* Authentication & Role-based access control (Admin, Manager, Cashier, Warehouse)
* Product Catalog, Barcode support, Unit conversions
* Sales, SaleItem, Discounts, Returns, Void
* Purchase Orders, Receiving, InventoryTransaction auto-creation
* Reorder alerts, Low-stock notifications
* VAT (TaxType: VATABLE, EXEMPT, ZERO\_RATED) breakdown
* Z/X Reading, Receipt numbering, Reprint logs, Audit logs

## Contributing

1. Fork the repo
2. Create a feature branch: `feature/your-feature`
3. Implement & add tests
4. Open PR and assign reviewer

Follow coding conventions: C# coding style, ESLint + Prettier on frontend, unit tests for critical services.

## License

MIT

---
