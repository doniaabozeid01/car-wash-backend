namespace carwash.Data.Entities;

public class WashRecord
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public int CarId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string CarType { get; set; } = string.Empty;
    public CarSize CarSize { get; set; }
    public int WashServiceId { get; set; }
    public string ServiceNameAr { get; set; } = string.Empty;
    public string ServiceNameEn { get; set; } = string.Empty;
    public int PointsChange { get; set; }
    public int CarPointsAfter { get; set; }
    public decimal AmountPaid { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
}
