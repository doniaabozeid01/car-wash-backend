namespace carwash.Data.Entities;

public class UserCampaignReceipt
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public Guid CampaignId { get; set; }
    public PointsCampaign Campaign { get; set; } = null!;
    public int PointsAdded { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
