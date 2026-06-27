namespace carwash.Service.DTOs.Points;

public class PointsUpdatedDto
{
    public int Points { get; set; }
    public int Change { get; set; }
    public string Action { get; set; } = string.Empty;
}
