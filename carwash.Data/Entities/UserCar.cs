namespace carwash.Data.Entities;

public class UserCar
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public string CarType { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public CarSize Size { get; set; }
    public int Points { get; set; }
}
