using carwash.Data;
using carwash.Data.Constants;
using carwash.Data.Entities;
using carwash.Service.DTOs.Cars;
using carwash.Service.DTOs.Common;
using carwash.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class UserCarService : IUserCarService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserCarService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<ServiceResult<IReadOnlyList<UserCarDto>>> GetAllAsync(string userId)
    {
        var customerResult = await ValidateCustomerAsync(userId);
        if (!customerResult.Success)
        {
            return ServiceResult<IReadOnlyList<UserCarDto>>.Fail(customerResult.Error!);
        }

        var cars = await _dbContext.UserCars
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Id)
            .Select(c => new UserCarDto
            {
                Id = c.Id,
                CarType = c.CarType,
                PlateNumber = c.PlateNumber,
                Size = c.Size,
                Points = c.Points
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<UserCarDto>>.Ok(cars);
    }

    public async Task<ServiceResult<UserCarDto>> GetByIdAsync(string userId, int carId)
    {
        var customerResult = await ValidateCustomerAsync(userId);
        if (!customerResult.Success)
        {
            return ServiceResult<UserCarDto>.Fail(customerResult.Error!);
        }

        var car = await FindUserCarAsync(userId, carId);
        if (car is null)
        {
            return ServiceResult<UserCarDto>.Fail("Car not found.");
        }

        return ServiceResult<UserCarDto>.Ok(MapToDto(car));
    }

    public async Task<ServiceResult<UserCarDto>> CreateAsync(CreateUserCarRequest request)
    {
        var customerResult = await ValidateCustomerAsync(request.UserId);
        if (!customerResult.Success)
        {
            return ServiceResult<UserCarDto>.Fail(customerResult.Error!);
        }

        if (await PlateNumberExistsAsync(request.UserId, request.PlateNumber))
        {
            return ServiceResult<UserCarDto>.Fail("A car with this plate number already exists.");
        }

        var car = new UserCar
        {
            UserId = request.UserId,
            CarType = request.CarType.Trim(),
            PlateNumber = request.PlateNumber.Trim(),
            Size = request.Size
        };

        _dbContext.UserCars.Add(car);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<UserCarDto>.Ok(MapToDto(car));
    }

    public async Task<ServiceResult<UserCarDto>> UpdateAsync(int carId, UpdateUserCarRequest request)
    {
        var customerResult = await ValidateCustomerAsync(request.UserId);
        if (!customerResult.Success)
        {
            return ServiceResult<UserCarDto>.Fail(customerResult.Error!);
        }

        var car = await FindUserCarAsync(request.UserId, carId);
        if (car is null)
        {
            return ServiceResult<UserCarDto>.Fail("Car not found.");
        }

        var plateNumber = request.PlateNumber.Trim();
        if (!string.Equals(car.PlateNumber, plateNumber, StringComparison.OrdinalIgnoreCase)
            && await PlateNumberExistsAsync(request.UserId, plateNumber))
        {
            return ServiceResult<UserCarDto>.Fail("A car with this plate number already exists.");
        }

        car.CarType = request.CarType.Trim();
        car.PlateNumber = plateNumber;
        car.Size = request.Size;

        await _dbContext.SaveChangesAsync();

        return ServiceResult<UserCarDto>.Ok(MapToDto(car));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string userId, int carId)
    {
        var customerResult = await ValidateCustomerAsync(userId);
        if (!customerResult.Success)
        {
            return ServiceResult<bool>.Fail(customerResult.Error!);
        }

        var car = await FindUserCarAsync(userId, carId);
        if (car is null)
        {
            return ServiceResult<bool>.Fail("Car not found.");
        }

        _dbContext.UserCars.Remove(car);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private async Task<ServiceResult<ApplicationUser>> ValidateCustomerAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return ServiceResult<ApplicationUser>.Fail("User not found.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<ApplicationUser>.Fail("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Roles.User))
        {
            return ServiceResult<ApplicationUser>.Fail("User not found.");
        }

        return ServiceResult<ApplicationUser>.Ok(user);
    }

    private async Task<UserCar?> FindUserCarAsync(string userId, int carId) =>
        await _dbContext.UserCars.FirstOrDefaultAsync(c => c.Id == carId && c.UserId == userId);

    private async Task<bool> PlateNumberExistsAsync(string userId, string plateNumber) =>
        await _dbContext.UserCars.AnyAsync(c =>
            c.UserId == userId && c.PlateNumber == plateNumber);

    private static UserCarDto MapToDto(UserCar car) => new()
    {
        Id = car.Id,
        CarType = car.CarType,
        PlateNumber = car.PlateNumber,
        Size = car.Size,
        Points = car.Points
    };
}
