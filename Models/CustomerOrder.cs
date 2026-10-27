using System.ComponentModel.DataAnnotations;

namespace ATPApi.Models;

public class CustomerOrder
{
    public int Id { get; set; }
    [Required, MaxLength(50)] public string OrderNumber { get; set; } = "";
    [Required, MaxLength(50)] public string ProductCode { get; set; } = "";
    [Required, MaxLength(50)] public string LocationCode { get; set; } = "";
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    public DateTime RequiredDate { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "Open";
}
