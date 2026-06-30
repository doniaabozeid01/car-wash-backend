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
        int? month = null)
    {
        HashSet<string>? activeUserIds = null;

        if (activeOnly)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;

            var rangeResult = GetMonthRange(targetYear, targetMonth);
            if (!rangeResult.Success)
            {
                return ServiceResult<IReadOnlyList<CustomerDto>>.Fail(rangeResult.Error!);
            }

            var (from, to) = rangeResult.Data;

            var activeUserIdList = await _dbContext.WashRecords
                .AsNoTracking()
                .Where(r => r.CreatedAt >= from && r.CreatedAt < to)
                .Select(r => r.UserId)
                .Distinct()
                .ToListAsync();

            activeUserIds = activeUserIdList.ToHashSet();
        }

        var customers = await _userManager.GetUsersInRoleAsync(Roles.User);

        IEnumerable<ApplicationUser> filtered = customers;
        if (activeUserIds is not null)
        {
            filtered = customers.Where(u => activeUserIds.Contains(u.Id));
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
