using System.ComponentModel.DataAnnotations;

namespace ATPApi.DTOs;

public class CheckAtpRequest
{
    [Required] public string ProductCode { get; set; } = "";
    [Required] public string LocationCode { get; set; } = "";
    [Range(1, int.MaxValue)] public int RequestedQuantity { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.Today;
}
