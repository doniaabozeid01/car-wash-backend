namespace carwash.Service.DTOs.WashRecords;

public class WashRecordStatsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalWashes { get; set; }
    public int FreeWashes { get; set; }
    public int UniqueCars { get; set; }
    public int UniqueCustomers { get; set; }
    public IReadOnlyList<WashRecordServiceStatDto> ByService { get; set; } = [];
}
