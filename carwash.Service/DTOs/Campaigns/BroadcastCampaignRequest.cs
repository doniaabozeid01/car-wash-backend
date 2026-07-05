namespace carwash.Service.DTOs.Campaigns;

public class BroadcastCampaignRequest
{
    public int Points { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }

    /// <summary>
    /// Optional. When omitted or empty, all customers receive the campaign.
    /// </summary>
    public List<string>? UserIds { get; set; }
}
