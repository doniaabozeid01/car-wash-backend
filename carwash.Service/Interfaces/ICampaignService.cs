using carwash.Service.DTOs.Campaigns;
using carwash.Service.DTOs.Common;

namespace carwash.Service.Interfaces;

public interface ICampaignService
{
    Task<ServiceResult<BroadcastCampaignResponse>> BroadcastAsync(string cashierId, BroadcastCampaignRequest request);
}
