using carwash.Data;
using carwash.Data.Entities;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.WashRecords;
using carwash.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class WashRecordService : IWashRecordService
{
    private readonly ApplicationDbContext _dbContext;

    public WashRecordService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<WashRecordsListDto>> GetRecordsAsync(
        int year,
        int month,
        string? userId = null,
        int? carId = null,
        int? washServiceId = null)
    {
        var rangeResult = GetMonthRange(year, month);
        if (!rangeResult.Success)
        {
            return ServiceResult<WashRecordsListDto>.Fail(rangeResult.Error!);
        }

        var (from, to) = rangeResult.Data;

        var query = _dbContext.WashRecords
            .AsNoTracking()
            .Where(r => r.CreatedAt >= from && r.CreatedAt < to);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(r => r.UserId == userId);
        }

        if (carId is not null)
        {
            query = query.Where(r => r.CarId == carId);
        }

        if (washServiceId is not null)
        {
            query = query.Where(r => r.WashServiceId == washServiceId);
        }

        var records = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new WashRecordDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = r.UserFullName,
                CarId = r.CarId,
                PlateNumber = r.PlateNumber,
                CarType = r.CarType,
                CarSize = r.CarSize,
                WashServiceId = r.WashServiceId,
                ServiceNameAr = r.ServiceNameAr,
                ServiceNameEn = r.ServiceNameEn,
                PointsChange = r.PointsChange,
                CarPointsAfter = r.CarPointsAfter,
                AmountPaid = r.AmountPaid,
                PaymentMethod = r.PaymentMethod,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        var response = new WashRecordsListDto
        {
            TotalAmount = records.Sum(r => r.AmountPaid),
            CashCount = records.Count(r => r.PaymentMethod == PaymentMethod.Cash),
            NetworkCount = records.Count(r => r.PaymentMethod == PaymentMethod.Network),
            Records = records
        };

        return ServiceResult<WashRecordsListDto>.Ok(response);
    }

    public async Task<ServiceResult<WashRecordStatsDto>> GetMonthlyStatsAsync(int year, int month)
    {
        var rangeResult = GetMonthRange(year, month);
        if (!rangeResult.Success)
        {
            return ServiceResult<WashRecordStatsDto>.Fail(rangeResult.Error!);
        }

        var (from, to) = rangeResult.Data;

        var records = await _dbContext.WashRecords
            .AsNoTracking()
            .Where(r => r.CreatedAt >= from && r.CreatedAt < to)
            .ToListAsync();

        var byService = records
            .GroupBy(r => new { r.WashServiceId, r.ServiceNameAr, r.ServiceNameEn })
            .Select(g => new WashRecordServiceStatDto
            {
                WashServiceId = g.Key.WashServiceId,
                ServiceNameAr = g.Key.ServiceNameAr,
                ServiceNameEn = g.Key.ServiceNameEn,
                Count = g.Count()
            })
            .OrderByDescending(s => s.Count)
            .ToList();

        var stats = new WashRecordStatsDto
        {
            Year = year,
            Month = month,
            TotalWashes = records.Count,
            FreeWashes = records.Count(r => r.PointsChange < 0),
            UniqueCars = records.Select(r => r.CarId).Distinct().Count(),
            UniqueCustomers = records.Select(r => r.UserId).Distinct().Count(),
            ByService = byService
        };

        return ServiceResult<WashRecordStatsDto>.Ok(stats);
    }

    private static ServiceResult<(DateTime From, DateTime To)> GetMonthRange(int year, int month)
    {
        if (year < 2000 || year > 9999)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid year.");
        }

        if (month is < 1 or > 12)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid month.");
        }

        var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        return ServiceResult<(DateTime, DateTime)>.Ok((from, from.AddMonths(1)));
    }
}
