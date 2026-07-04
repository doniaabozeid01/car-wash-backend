namespace carwash.Service.DTOs.Notifications;

public class CampaignNotificationDto
{
    public Guid ReceiptId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Points { get; set; }
}
