using System.Security.Claims;
using carwash.Data.Constants;
using carwash.Service.DTOs.Campaigns;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/campaigns")]
[Authorize(Roles = Roles.Cashier)]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastCampaignRequest request)
    {
        var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(cashierId))
        {
            return Unauthorized();
        }

        var result = await _campaignService.BroadcastAsync(cashierId, request);
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    private IActionResult ToActionResult<T>(ServiceResult<T> result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.Success)
        {
            return StatusCode(successStatusCode, result.Data);
        }

        if (result.Errors.Any())
        {
            return BadRequest(new { message = result.Error, errors = result.Errors });
        }

        return BadRequest(new { message = result.Error });
    }
}
