namespace carwash.Service.DTOs.WashRecords;

public class WashRecordServiceStatDto
{
    public int WashServiceId { get; set; }
    public string ServiceNameAr { get; set; } = string.Empty;
    public string ServiceNameEn { get; set; } = string.Empty;
    public int Count { get; set; }
}
