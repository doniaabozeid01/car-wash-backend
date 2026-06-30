using carwash.Data;
using carwash.Data.Constants;
using carwash.Data.Entities;
using carwash.Service.DTOs.Cars;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Points;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class PointsService : IPointsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPointsNotifier _pointsNotifier;

    public PointsService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IPointsNotifier pointsNotifier)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _pointsNotifier = pointsNotifier;
    }

    public async Task<ServiceResult<ScannedUserDto>> GetUserByQrCodeAsync(string qrCode)
    {
        var user = await FindCustomerByQrCodeAsync(qrCode);
        if (user is null)
        {
            return ServiceResult<ScannedUserDto>.Fail("User not found for this QR code.");
        }

        return ServiceResult<ScannedUserDto>.Ok(await MapToDtoAsync(user));
    }

    public async Task<ServiceResult<ScannedUserDto>> ApplyPointsAsync(ApplyPointsRequest request)
    {
        var user = await FindCustomerByQrCodeAsync(request.QrCode);
        if (user is null)
        {
            return ServiceResult<ScannedUserDto>.Fail("User not found for this QR code.");
        }

        var car = await _dbContext.UserCars
            .FirstOrDefaultAsync(c => c.Id == request.CarId && c.UserId == user.Id);
        if (car is null)
        {
            return ServiceResult<ScannedUserDto>.Fail("Car not found.");
        }

        var washService = await _dbContext.WashServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId);
        if (washService is null)
        {
            return ServiceResult<ScannedUserDto>.Fail("Service not found.");
        }

        var change = washService.Points;

        if (change < 0 && car.Points + change < 0)
        {
            var required = Math.Abs(change);
            var remaining = required - car.Points;
            return ServiceResult<ScannedUserDto>.Fail(
                $"Insufficient points. You still need {remaining} more points to redeem {required} points.");
        }

        decimal amountPaid = 0;
        PaymentMethod? paymentMethod = null;

        if (change > 0)
        {
            if (request.AmountPaid <= 0)
            {
                return ServiceResult<ScannedUserDto>.Fail("Amount paid is required for this service.");
            }

            if (request.PaymentMethod is null)
            {
                return ServiceResult<ScannedUserDto>.Fail("Payment method is required for this service.");
            }

            amountPaid = request.AmountPaid;
            paymentMethod = request.PaymentMethod;
        }

        car.Points += change;

        _dbContext.WashRecords.Add(new WashRecord
        {
            UserId = user.Id,
            UserFullName = user.FullName,
            CarId = car.Id,
            PlateNumber = car.PlateNumber,
            CarType = car.CarType,
            CarSize = car.Size,
            WashServiceId = washService.Id,
            ServiceNameAr = washService.NameAr,
            ServiceNameEn = washService.NameEn,
            PointsChange = change,
            CarPointsAfter = car.Points,
            AmountPaid = amountPaid,
            PaymentMethod = paymentMethod,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        await _pointsNotifier.NotifyPointsUpdatedAsync(user.Id, new PointsUpdatedDto
        {
            CarId = car.Id,
            PlateNumber = car.PlateNumber,
            Points = car.Points,
            Change = change,
            ServiceNameAr = washService.NameAr,
            ServiceNameEn = washService.NameEn
        });

        return ServiceResult<ScannedUserDto>.Ok(await MapToDtoAsync(user));
    }

    private async Task<ApplicationUser?> FindCustomerByQrCodeAsync(string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
        {
            return null;
        }

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.QrCode == qrCode);

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Roles.User))
        {
            return null;
        }

        return user;
    }

    private async Task<ScannedUserDto> MapToDtoAsync(ApplicationUser user)
    {
        var cars = await _dbContext.UserCars
            .AsNoTracking()
            .Where(c => c.UserId == user.Id)
            .OrderByDescending(c => c.Id)
            .Select(c => new UserCarDto
            {
                Id = c.Id,
                CarType = c.CarType,
                PlateNumber = c.PlateNumber,
                Size = c.Size,
                Points = c.Points
            })
            .ToListAsync();

        return new ScannedUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Cars = cars
        };
    }
}
