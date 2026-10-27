# ATP API (Available-to-Promise) — ASP.NET Core 8 + SQL Server

This is an original sample ATP API project for learning/interview demonstration.

## Business logic
ATP = max(0, On-hand inventory - Reserved inventory + eligible incoming supply - open demand due on/before the requested date).

For production-grade systems, calculate ATP in date buckets and account for order allocation rules, supply status, lead times, concurrency/transactions, unit of measure, and permissions. This sample keeps the logic intentionally easy to understand.

## Requirements
- .NET 8 SDK
- SQL Server / SQL Server Express / LocalDB
- Visual Studio 2022

## Setup
1. Open `ATPApi.csproj` in Visual Studio.
2. Run `Database/ATPPlanningDb.sql` in SQL Server Management Studio. If using a SQL Server instance other than LocalDB, update `ConnectionStrings:DefaultConnection` in `appsettings.json`.
3. In Visual Studio, restore NuGet packages.
4. Run the project (F5). Swagger opens in Development mode.

## Example endpoints
- `GET /api/atp?productCode=SKU-1001&locationCode=MUM-WH1&requestedDate=2026-10-30`
- `POST /api/atp/check`
- `GET /api/atp/inventory`
- `POST /api/atp/inventory`
- `GET /api/atp/incoming-supply`
- `POST /api/atp/incoming-supply`
- `GET /api/atp/orders`
- `POST /api/atp/orders`

### POST check sample JSON
```json
{
  "productCode": "SKU-1001",
  "locationCode": "MUM-WH1",
  "requestedQuantity": 25,
  "requestedDate": "2026-10-30T00:00:00"
}
```

### GET ATP example
For the sample rows, if the requested date is today, available on-hand is 100 - 20 = 80. Eligible incoming supply is 0 until its expected date. Eligible open demand is 15. ATP = 80 - 15 = 65.

## Notes
- This project uses `EnsureCreated` nowhere; the supplied SQL script creates the database/tables.
- The table names and columns are aligned with the EF Core models.
- If you use Visual Studio's IIS Express profile, copy the actual Swagger URL from the browser or Output window.
