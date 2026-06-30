using carwash.Service.DTOs.Cars;
using carwash.Service.DTOs.Common;

namespace carwash.Service.Interfaces;

public interface IUserCarService
{
    Task<ServiceResult<IReadOnlyList<UserCarDto>>> GetAllAsync(string userId);
    Task<ServiceResult<UserCarDto>> GetByIdAsync(string userId, int carId);
    Task<ServiceResult<UserCarDto>> CreateAsync(CreateUserCarRequest request);
    Task<ServiceResult<UserCarDto>> UpdateAsync(int carId, UpdateUserCarRequest request);
    Task<ServiceResult<bool>> DeleteAsync(string userId, int carId);
}
