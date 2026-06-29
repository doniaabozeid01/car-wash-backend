using carwash.Data.Constants;
using carwash.Data.Entities;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Points;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class PointsService : IPointsService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPointsNotifier _pointsNotifier;

    public PointsService(UserManager<ApplicationUser> userManager, IPointsNotifier pointsNotifier)
    {
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

        return ServiceResult<ScannedUserDto>.Ok(MapToDto(user));
    }

    public async Task<ServiceResult<ScannedUserDto>> ApplyPointsAsync(ApplyPointsRequest request)
    {
        var user = await FindCustomerByQrCodeAsync(request.QrCode);
        if (user is null)
        {
            return ServiceResult<ScannedUserDto>.Fail("User not found for this QR code.");
        }

        var change = PointsActionValues.GetChange(request.Action);

        if (change < 0 && user.Points + change < 0)
        {
            var required = Math.Abs(change);
            var remaining = required - user.Points;
            return ServiceResult<ScannedUserDto>.Fail(
                $"Insufficient points. You still need {remaining} more points to redeem {required} points.");
        }

        user.Points += change;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return ServiceResult<ScannedUserDto>.Fail(updateResult.Errors.Select(e => e.Description));
        }

        await _pointsNotifier.NotifyPointsUpdatedAsync(user.Id, new PointsUpdatedDto
        {
            Points = user.Points,
            Change = change,
            Action = request.Action.ToString()
        });

        return ServiceResult<ScannedUserDto>.Ok(MapToDto(user));
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

    private static ScannedUserDto MapToDto(ApplicationUser user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber ?? string.Empty,
        Points = user.Points
    };
}
