using carwash.Data;
using carwash.Data.Constants;
using carwash.Data.Entities;
using carwash.Service.DTOs.Campaigns;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Notifications;
using carwash.Service.DTOs.Points;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class CampaignService : ICampaignService
{
    private const string DefaultTitle = "تم تحديث نقاطك";
    private const string DefaultMessageTemplate = "تم إضافة {0} نقطة إلى حسابك.";

    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPointsNotifier _pointsNotifier;

    public CampaignService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IPointsNotifier pointsNotifier)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _pointsNotifier = pointsNotifier;
    }

    public async Task<ServiceResult<BroadcastCampaignResponse>> BroadcastAsync(
        string cashierId,
        BroadcastCampaignRequest request)
    {
        if (request.Points <= 0)
        {
            return ServiceResult<BroadcastCampaignResponse>.Fail("Points must be greater than zero.");
        }

        var title = string.IsNullOrWhiteSpace(request.Title) ? DefaultTitle : request.Title.Trim();
        var message = string.IsNullOrWhiteSpace(request.Message)
            ? string.Format(DefaultMessageTemplate, request.Points)
            : request.Message.Trim();

        var customerIds = await (
            from user in _userManager.Users
            join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
            join role in _dbContext.Roles on userRole.RoleId equals role.Id
            where role.Name == Roles.User
            select user.Id
        ).ToListAsync();

        if (customerIds.Count == 0)
        {
            return ServiceResult<BroadcastCampaignResponse>.Fail("No customers found to notify.");
        }

        var cars = await _dbContext.UserCars
            .Where(c => customerIds.Contains(c.UserId))
            .OrderBy(c => c.Id)
            .ToListAsync();

        var carsByUser = cars
            .GroupBy(c => c.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var campaign = new PointsCampaign
        {
            Id = Guid.NewGuid(),
            PointsAmount = request.Points,
            Title = title,
            Message = message,
            CreatedByCashierId = cashierId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.PointsCampaigns.Add(campaign);

        var receipts = new List<UserCampaignReceipt>();
        var pointsUpdates = new List<(string UserId, UserCar Car, UserCampaignReceipt Receipt)>();

        foreach (var userId in customerIds)
        {
            var receipt = new UserCampaignReceipt
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CampaignId = campaign.Id,
                PointsAdded = request.Points,
                IsRead = false
            };

            receipts.Add(receipt);

            if (carsByUser.TryGetValue(userId, out var userCars))
            {
                foreach (var car in userCars)
                {
                    car.Points += request.Points;
                    pointsUpdates.Add((userId, car, receipt));
                }
            }
        }

        _dbContext.UserCampaignReceipts.AddRange(receipts);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var campaignNotifiedUsers = new HashSet<string>();

        foreach (var (userId, car, receipt) in pointsUpdates)
        {
            await _pointsNotifier.NotifyPointsUpdatedAsync(userId, new PointsUpdatedDto
            {
                CarId = car.Id,
                PlateNumber = car.PlateNumber,
                Points = car.Points,
                Change = request.Points,
                ServiceNameAr = title,
                ServiceNameEn = title
            });

            if (campaignNotifiedUsers.Add(userId))
            {
                await _pointsNotifier.NotifyCampaignNotificationAsync(userId, new CampaignNotificationDto
                {
                    ReceiptId = receipt.Id,
                    Title = title,
                    Message = message,
                    Points = request.Points
                });
            }
        }

        foreach (var receipt in receipts.Where(r => !campaignNotifiedUsers.Contains(r.UserId)))
        {
            await _pointsNotifier.NotifyCampaignNotificationAsync(receipt.UserId, new CampaignNotificationDto
            {
                ReceiptId = receipt.Id,
                Title = title,
                Message = message,
                Points = request.Points
            });
        }

        return ServiceResult<BroadcastCampaignResponse>.Ok(new BroadcastCampaignResponse
        {
            CampaignId = campaign.Id,
            PointsAmount = campaign.PointsAmount,
            Title = campaign.Title,
            Message = campaign.Message,
            UsersNotified = receipts.Count,
            CreatedAt = campaign.CreatedAt
        });
    }
}
