using ATPApi.DTOs;

namespace ATPApi.Services;

public interface IAtpService
{
    Task<AtpResponse> GetAtpAsync(string productCode, string locationCode, DateTime requestedDate);
    Task<object> CheckAvailabilityAsync(CheckAtpRequest request);
}
