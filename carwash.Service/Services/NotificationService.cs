using carwash.Data;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Notifications;
using carwash.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<NotificationDto?>> GetUnreadAsync(string userId)
    {
        var receipt = await _dbContext.UserCampaignReceipts
            .AsNoTracking()
            .Include(r => r.Campaign)
            .Where(r => r.UserId == userId && !r.IsRead)
            .OrderBy(r => r.Campaign.CreatedAt)
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            return ServiceResult<NotificationDto?>.Ok(null);
        }

        return ServiceResult<NotificationDto?>.Ok(MapToDto(receipt));
    }

    public async Task<ServiceResult<NotificationDto>> MarkAsReadAsync(string userId, Guid receiptId)
    {
        var receipt = await _dbContext.UserCampaignReceipts
            .Include(r => r.Campaign)
            .FirstOrDefaultAsync(r => r.Id == receiptId && r.UserId == userId);

        if (receipt is null)
        {
            return ServiceResult<NotificationDto>.Fail("Notification not found.");
        }

        if (!receipt.IsRead)
        {
            receipt.IsRead = true;
            receipt.ReadAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return ServiceResult<NotificationDto>.Ok(MapToDto(receipt));
    }

    public async Task<ServiceResult<IReadOnlyList<NotificationDto>>> GetAllAsync(string userId)
    {
        var receipts = await _dbContext.UserCampaignReceipts
            .AsNoTracking()
            .Include(r => r.Campaign)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Campaign.CreatedAt)
            .ToListAsync();

        return ServiceResult<IReadOnlyList<NotificationDto>>.Ok(
            receipts.Select(MapToDto).ToList());
    }

    private static NotificationDto MapToDto(Data.Entities.UserCampaignReceipt receipt) =>
        new()
        {
            Id = receipt.Id,
            Title = receipt.Campaign.Title,
            Message = receipt.Campaign.Message,
            PointsAdded = receipt.PointsAdded,
            IsRead = receipt.IsRead,
            CreatedAt = receipt.Campaign.CreatedAt,
            ReadAt = receipt.ReadAt
        };
}
