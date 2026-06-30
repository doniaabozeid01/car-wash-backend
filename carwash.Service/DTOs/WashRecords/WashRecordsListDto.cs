namespace carwash.Service.DTOs.WashRecords;

public class WashRecordsListDto
{
    public decimal TotalAmount { get; set; }
    public int CashCount { get; set; }
    public int NetworkCount { get; set; }
    public IReadOnlyList<WashRecordDto> Records { get; set; } = [];
}
