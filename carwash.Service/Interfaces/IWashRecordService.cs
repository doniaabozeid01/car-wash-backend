using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.WashRecords;

namespace carwash.Service.Interfaces;

public interface IWashRecordService
{
    Task<ServiceResult<WashRecordsListDto>> GetRecordsAsync(
        int year,
        int month,
        string? userId = null,
        int? carId = null,
        int? washServiceId = null);

    Task<ServiceResult<WashRecordStatsDto>> GetMonthlyStatsAsync(int year, int month);
}
