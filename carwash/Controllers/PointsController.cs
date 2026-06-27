using carwash.Data.Constants;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Points;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Cashier)]
public class PointsController : ControllerBase
{
    private readonly IPointsService _pointsService;

    public PointsController(IPointsService pointsService)
    {
        _pointsService = pointsService;
    }

    [HttpGet("scan/{qrCode}")]
    public async Task<IActionResult> ScanQrCode(string qrCode)
    {
        var result = await _pointsService.GetUserByQrCodeAsync(qrCode);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    [HttpPost("apply")]
    public async Task<IActionResult> ApplyPoints([FromBody] ApplyPointsRequest request)
    {
        var result = await _pointsService.ApplyPointsAsync(request);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    private IActionResult ToActionResult<T>(ServiceResult<T> result, int notFoundStatusCode)
    {
        if (result.Success)
        {
            return Ok(result.Data);
        }

        if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
        {
            return StatusCode(notFoundStatusCode, new { message = result.Error });
        }

        if (result.Errors.Any())
        {
            return BadRequest(new { message = result.Error, errors = result.Errors });
        }

        return BadRequest(new { message = result.Error });
    }
}
