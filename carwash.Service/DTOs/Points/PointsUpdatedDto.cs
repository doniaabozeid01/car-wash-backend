namespace carwash.Service.DTOs.Points;

public class PointsUpdatedDto
{
    public int CarId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Change { get; set; }
    public string ServiceNameAr { get; set; } = string.Empty;
    public string ServiceNameEn { get; set; } = string.Empty;
}
