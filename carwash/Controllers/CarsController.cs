using carwash.Data.Constants;
using carwash.Service.DTOs.Cars;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly IUserCarService _userCarService;

    public CarsController(IUserCarService userCarService)
    {
        _userCarService = userCarService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string userId)
    {
        var result = await _userCarService.GetAllAsync(userId);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> GetById(int id, [FromQuery] string userId)
    {
        var result = await _userCarService.GetByIdAsync(userId, id);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Create([FromBody] CreateUserCarRequest request)
    {
        var result = await _userCarService.CreateAsync(request);
        return ToActionResult(result, StatusCodes.Status404NotFound, StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCarRequest request)
    {
        var result = await _userCarService.UpdateAsync(id, request);
        return ToActionResult(result, StatusCodes.Status404NotFound);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Cashier)]
    public async Task<IActionResult> Delete(int id, [FromQuery] string userId)
    {
        var result = await _userCarService.DeleteAsync(userId, id);
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
