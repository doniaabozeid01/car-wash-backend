using carwash.Data.Constants;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.WashServices;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/wash-services")]
public class WashServicesController : ControllerBase
{
    private readonly IWashServicesService _washServicesService;

    public WashServicesController(IWashServicesService washServicesService)
    {
        _washServicesService = washServicesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _washServicesService.GetAllAsync();
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _washServicesService.GetByIdAsync(id);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Create([FromBody] CreateWashServiceRequest request)
    {
        var result = await _washServicesService.CreateAsync(request);
        return ToActionResult(result, StatusCodes.Status404NotFound, StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWashServiceRequest request)
    {
        var result = await _washServicesService.UpdateAsync(id, request);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _washServicesService.DeleteAsync(id);
        return ToActionResult(result, StatusCodes.Status404NotFound, successStatusCode: StatusCodes.Status204NoContent);
    }

    private IActionResult ToActionResult<T>(
        ServiceResult<T> result,
        int notFoundStatusCode = StatusCodes.Status404NotFound,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.Success)
        {
            if (successStatusCode == StatusCodes.Status204NoContent)
            {
                return NoContent();
            }

            return StatusCode(successStatusCode, result.Data);
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
