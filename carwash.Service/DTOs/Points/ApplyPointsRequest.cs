using System.ComponentModel.DataAnnotations;
using carwash.Data.Entities;

namespace carwash.Service.DTOs.Points;

public class ApplyPointsRequest
{
    [Required]
    public string QrCode { get; set; } = string.Empty;

    [Required]
    public int ServiceId { get; set; }

    [Required]
    public int CarId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }
}
