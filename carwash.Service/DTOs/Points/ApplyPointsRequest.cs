using System.ComponentModel.DataAnnotations;

namespace carwash.Service.DTOs.Points;

public class ApplyPointsRequest
{
    [Required]
    public string QrCode { get; set; } = string.Empty;

    [Required]
    public PointsActionType Action { get; set; }
}
