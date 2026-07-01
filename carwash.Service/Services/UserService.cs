using carwash.Data;
using carwash.Data.Constants;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Users;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApplicationUser = carwash.Data.Entities.ApplicationUser;

namespace carwash.Service.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<ServiceResult<IReadOnlyList<CustomerDto>>> GetCustomersAsync(
        bool activeOnly = false,
        int? year = null,
        int? month = null,
        int? day = null)
    {
        var hasDateFilter = year.HasValue || month.HasValue || day.HasValue;

        var customers = await _userManager.GetUsersInRoleAsync(Roles.User);
        var cashierIds = (await _userManager.GetUsersInRoleAsync(Roles.Cashier))
            .Select(u => u.Id)
            .ToHashSet();

        IEnumerable<ApplicationUser> filtered = customers
            .Where(u => !cashierIds.Contains(u.Id))
            .Where(u => !string.IsNullOrEmpty(u.QrCode));

        if (activeOnly || hasDateFilter)
        {
            var rangeResult = ResolveDateRange(
                year,
                month,
                day,
                defaultToCurrentMonth: activeOnly && !hasDateFilter);
            if (!rangeResult.Success)
            {
                return ServiceResult<IReadOnlyList<CustomerDto>>.Fail(rangeResult.Error!);
            }

            var (from, to) = rangeResult.Data;

            if (activeOnly)
            {
                var activeUserIds = (await _dbContext.WashRecords
                    .AsNoTracking()
                    .Where(r => r.CreatedAt >= from && r.CreatedAt < to)
                    .Select(r => r.UserId)
                    .Distinct()
                    .ToListAsync()).ToHashSet();

                filtered = filtered.Where(u => activeUserIds.Contains(u.Id));
            }
            else
            {
                filtered = filtered.Where(u => u.CreatedAt >= from && u.CreatedAt < to);
            }
        }

        var result = filtered
            .OrderBy(u => u.FullName)
            .Select(u => new CustomerDto
            {
                Id = u.Id,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber ?? string.Empty
            })
            .ToList();

        return ServiceResult<IReadOnlyList<CustomerDto>>.Ok(result);
    }

    public async Task<ServiceResult<bool>> DeleteCustomerAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return ServiceResult<bool>.Fail("User not found.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<bool>.Fail("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Roles.User))
        {
            return ServiceResult<bool>.Fail("Only customer accounts can be deleted.");
        }

        await _dbContext.WashRecords
            .Where(r => r.UserId == userId)
            .ExecuteDeleteAsync();

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            return ServiceResult<bool>.Fail(deleteResult.Errors.Select(e => e.Description));
        }

        return ServiceResult<bool>.Ok(true);
    }

    private static ServiceResult<(DateTime From, DateTime To)> ResolveDateRange(
        int? year,
        int? month,
        int? day,
        bool defaultToCurrentMonth = false)
    {
        var now = DateTime.UtcNow;

        if (day.HasValue)
        {
            if (!year.HasValue || !month.HasValue)
            {
                return ServiceResult<(DateTime, DateTime)>.Fail("Year and month are required when day is specified.");
            }

            return GetDayRange(year.Value, month.Value, day.Value);
        }

        if (month.HasValue)
        {
            return GetMonthRange(year ?? now.Year, month.Value);
        }

        if (year.HasValue)
        {
            return GetYearRange(year.Value);
        }

        if (defaultToCurrentMonth)
        {
            return GetMonthRange(now.Year, now.Month);
        }

        return ServiceResult<(DateTime, DateTime)>.Fail("Invalid date range.");
    }

    private static ServiceResult<(DateTime From, DateTime To)> GetYearRange(int year)
    {
        if (year < 2000 || year > 9999)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid year.");
        }

        var from = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return ServiceResult<(DateTime, DateTime)>.Ok((from, from.AddYears(1)));
    }

    private static ServiceResult<(DateTime From, DateTime To)> GetDayRange(int year, int month, int day)
    {
        if (year < 2000 || year > 9999)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid year.");
        }

        if (month is < 1 or > 12)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid month.");
        }

        if (day is < 1 or > 31)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid day.");
        }

        try
        {
            var from = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            return ServiceResult<(DateTime, DateTime)>.Ok((from, from.AddDays(1)));
        }
        catch (ArgumentOutOfRangeException)
        {
            return ServiceResult<(DateTime, DateTime)>.Fail("Invalid date.");
        }
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
