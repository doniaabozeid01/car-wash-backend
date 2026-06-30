using carwash.Data.Entities;

namespace carwash.Service.DTOs.Cars;

public class UserCarDto
{
    public int Id { get; set; }
    public string CarType { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public CarSize Size { get; set; }
    public int Points { get; set; }
}
