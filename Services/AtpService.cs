using ATPApi.Data;
using ATPApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ATPApi.Services;

public class AtpService : IAtpService
{
    private readonly AtpDbContext _db;
    public AtpService(AtpDbContext db) => _db = db;

    public async Task<AtpResponse> GetAtpAsync(string productCode, string locationCode, DateTime requestedDate)
    {
        productCode = productCode.Trim();
        locationCode = locationCode.Trim();
        var inventory = await _db.Inventories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductCode == productCode && x.LocationCode == locationCode);

        int onHand = inventory?.OnHandQuantity ?? 0;
        int reserved = inventory?.ReservedQuantity ?? 0;
        int availableOnHand = Math.Max(0, onHand - reserved);

        int incoming = await _db.IncomingSupplies.AsNoTracking()
            .Where(x => x.ProductCode == productCode && x.LocationCode == locationCode
                && x.Status == "Open" && x.ExpectedDate.Date <= requestedDate.Date)
            .SumAsync(x => (int?)x.Quantity) ?? 0;

        int openDemand = await _db.CustomerOrders.AsNoTracking()
            .Where(x => x.ProductCode == productCode && x.LocationCode == locationCode
                && x.Status == "Open" && x.RequiredDate.Date <= requestedDate.Date)
            .SumAsync(x => (int?)x.Quantity) ?? 0;

        int atp = Math.Max(0, availableOnHand + incoming - openDemand);
        return new AtpResponse
        {
            ProductCode = productCode,
            LocationCode = locationCode,
            RequestedDate = requestedDate,
            OnHandQuantity = onHand,
            ReservedQuantity = reserved,
            AvailableOnHand = availableOnHand,
            IncomingSupplyQuantity = incoming,
            OpenDemandQuantity = openDemand,
            AtpQuantity = atp,
            AvailabilityStatus = atp > 0 ? "Available" : "NotAvailable",
            Recommendation = atp > 0
                ? $"Up to {atp} unit(s) may be promised for the requested date."
                : "No available-to-promise quantity. Check later incoming supply or another location."
        };
    }

    public async Task<object> CheckAvailabilityAsync(CheckAtpRequest request)
    {
        var atp = await GetAtpAsync(request.ProductCode, request.LocationCode, request.RequestedDate);
        return new
        {
            atp.ProductCode,
            atp.LocationCode,
            atp.RequestedDate,
            request.RequestedQuantity,
            atp.AtpQuantity,
            CanFulfill = atp.AtpQuantity >= request.RequestedQuantity,
            Message = atp.AtpQuantity >= request.RequestedQuantity
                ? "Requested quantity can be promised."
                : $"Shortage of {request.RequestedQuantity - atp.AtpQuantity} unit(s)."
        };
    }
}
