namespace carwash.Service.DTOs.Campaigns;

public class BroadcastCampaignResponse
{
    public Guid CampaignId { get; set; }
    public int PointsAmount { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int UsersNotified { get; set; }
    public DateTime CreatedAt { get; set; }
}
