using System.ComponentModel.DataAnnotations;
using carwash.Data.Entities;

namespace carwash.Service.DTOs.Points;

public class ApplyManualPointsRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int CarId { get; set; }

    [Required]
    public int Points { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }
}