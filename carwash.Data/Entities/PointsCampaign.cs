namespace carwash.Data.Entities;

public class PointsCampaign
{
    public Guid Id { get; set; }
    public int PointsAmount { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CreatedByCashierId { get; set; } = string.Empty;
    public ApplicationUser CreatedByCashier { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserCampaignReceipt> Receipts { get; set; } = [];
}
