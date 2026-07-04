using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Notifications;

namespace carwash.Service.Interfaces;

public interface INotificationService
{
    Task<ServiceResult<NotificationDto?>> GetUnreadAsync(string userId);
    Task<ServiceResult<NotificationDto>> MarkAsReadAsync(string userId, Guid receiptId);
    Task<ServiceResult<IReadOnlyList<NotificationDto>>> GetAllAsync(string userId);
}
