using carwash.Data.Constants;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carwash.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Cashier)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool activeOnly = false,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null,
        [FromQuery] int? day = null)
    {
        var result = await _userService.GetCustomersAsync(activeOnly, year, month, day);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteCustomerAsync(id);
        return ToActionResult(result, StatusCodes.Status404NotFound, StatusCodes.Status204NoContent);
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
