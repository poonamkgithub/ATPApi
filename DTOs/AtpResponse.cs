namespace ATPApi.DTOs;

public class AtpResponse
{
    public string ProductCode { get; set; } = "";
    public string LocationCode { get; set; } = "";
    public DateTime RequestedDate { get; set; }
    public int OnHandQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableOnHand { get; set; }
    public int IncomingSupplyQuantity { get; set; }
    public int OpenDemandQuantity { get; set; }
    public int AtpQuantity { get; set; }
    public string AvailabilityStatus { get; set; } = "";
    public string Recommendation { get; set; } = "";
}
