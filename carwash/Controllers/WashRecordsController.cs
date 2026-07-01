using carwash.Data.Constants;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/wash-records")]
[Authorize(Roles = Roles.Cashier)]
public class WashRecordsController : ControllerBase
{
    private readonly IWashRecordService _washRecordService;

    public WashRecordsController(IWashRecordService washRecordService)
    {
        _washRecordService = washRecordService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecords(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int? day = null,
        [FromQuery] string? userId = null,
        [FromQuery] int? carId = null,
        [FromQuery] int? washServiceId = null)
    {
        var result = await _washRecordService.GetRecordsAsync(year, month, day, userId, carId, washServiceId);
        return ToActionResult(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetMonthlyStats(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int? day = null)
    {
        var result = await _washRecordService.GetMonthlyStatsAsync(year, month, day);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
        {
            return Ok(result.Data);
        }

        if (result.Errors.Any())
        {
            return BadRequest(new { message = result.Error, errors = result.Errors });
        }

        return BadRequest(new { message = result.Error });
    }
}
